using System.Collections.ObjectModel;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Model.ElementTypes;
using CAMCMSServer.Model.Requests;
using CAMCMSServer.Utils;
using Newtonsoft.Json.Linq;
using Npgsql;

namespace CAMCMSServer.Database.Service;

public interface IModuleService
{
    #region Methods

    Task<IEnumerable<Module>> GetAll();
    Task<IEnumerable<Module>> GetAllInLibraryFolder(LibraryFolder folder);
    Task<Module> GetByModuleId(int id);
    Task<Module> Create(Module module);
    Task Update(Module updatedElement);
    Task Delete(Module module);

    Task UpdateLibraryFolder(Module module);

    Task UpdateLibraryFolderWithElements(Module module);
    Task UpdateLocationAttribute(ElementLocation location);
    Task<bool> HasPublishedModule(Module module);

    #endregion

    Task<Module> GetModuleFromElementLocation(ElementLocation location);
}

public class ModuleService : IModuleService
{
    #region Data members

    private readonly IModuleRepository repository;
    private readonly IElementSetService setService;
    private readonly IElementService elementRepository;
    private readonly IPublishedModuleRepository publishedModuleRepository;
    //private readonly IFileService fileService;

    #endregion

    #region Constructors

    public ModuleService(IModuleRepository repository, IElementSetService sService, IElementService eRepository,
        IPublishedModuleRepository publishedModuleRepository)
    {
        this.repository = repository;
        this.setService = sService;
        this.elementRepository = eRepository;
        this.publishedModuleRepository = publishedModuleRepository;
        //this.fileService = fileService;
    }

    #endregion

    #region Methods

    public async Task<IEnumerable<Module>> GetAll()
    {
        var modules = await this.repository.GetAll();
        var enumerable = modules as Module[] ?? modules.ToArray();

        foreach (var module in enumerable)
        {
            module.ElementSets = new Collection<ElementSet>();
        }

        return enumerable;
    }

    public async Task<Module> GetByModuleId(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException("Id must be greater than 0");
        }

        var module = await this.repository.GetById(id, 1);
        var sets = await this.repository.GetElementSets(id);
        var locations = await this.repository.GetElementLocations(id);
        var elements = await this.repository.GetElementsByModule(id);

        elements.AsParallel().ForAll(x => this.loadElement(x));

        locations.AsParallel().ForAll(x =>
        {
            x.Element = elements.FirstOrDefault(y => y.ElementId == x.ElementId);
            x.Attributes ??= new SetLocationAttributes();
        });

        module.ElementSets = sets;
        module.ElementSets.AsParallel()
            .ForAll(x =>
            {
                x.Elements = locations.Where(y => y.SetLocationId == x.SetLocationId);
                x.Styling ??= new SetStyling();
            });
        var cache = GenerateCachedModule.GenerateCacheModule(module); 
        return module;
    }

    public async Task<IEnumerable<Module>> GetAllInLibraryFolder(LibraryFolder folder)
    {
        var modules = await this.repository.GetByLibraryFolderId(folder.LibraryFolderId);

        var enumerable = modules as Module[] ?? modules.ToArray();

        foreach (var module in enumerable)
        {
            module.ElementSets = new Collection<ElementSet>();
        }

        return enumerable;
    }

    public async Task<Module> Create(Module module)
    {
        if (module == null)
        {
            throw new ArgumentNullException(nameof(module));
        }

        module.DisplayTitle ??= module.Title;
        module.IsTemplate ??= false;

        await this.repository.Create(module);

        return await this.repository.GetByTitle(module.Title);
    }

    public async Task Update(Module updatedElement)
    {
        if (updatedElement == null)
        {
            throw new ArgumentNullException(nameof(updatedElement));
        }

        updatedElement.ElementSets ??= new List<ElementSet>();
        updatedElement.DisplayTitle ??= updatedElement.Title;
        updatedElement.IsTemplate ??= false;

        var currentModule = await this.repository.GetById(updatedElement.ModuleId, 1);
        currentModule.ElementSets = await this.setService.GetByModule(currentModule);
        currentModule.LibraryFolderId = updatedElement.LibraryFolderId;

        var currentModuleElementSets = currentModule.ElementSets.ToList();

        var updateError = new List<ElementSet>();
        foreach (var updatedElementSet in updatedElement.ElementSets)
        {
            if (updatedElementSet == null)
            {
                continue;
            }

            var found = currentModuleElementSets.Find(x => x != null && x.Equals(updatedElementSet));
            if (found != null)
            {
                try
                {
                    await this.setService.Update(updatedElementSet);
                }
                catch (PostgresException)
                {
                    updateError.Add(updatedElementSet);
                    var tempSet = new ElementSet
                    {
                        SetLocationId = updatedElementSet.SetLocationId,
                        ModuleId = updatedElementSet.ModuleId,
                        Place = int.MaxValue,
                        IsEditable = updatedElementSet.IsEditable,
                        Styling = new SetStyling
                        {
                            is_appendix = updatedElementSet.Styling?.is_appendix,
                            is_horizontal = updatedElementSet.Styling?.is_horizontal,
                            has_page_break = updatedElementSet.Styling?.has_page_break
                        }
                    };
                    await this.setService.Update(tempSet);
                }

                currentModuleElementSets.Remove(found);
            }
            else
            {
                updatedElementSet.ModuleId ??= updatedElement.ModuleId;
                await this.setService.Create(updatedElementSet);
            }
        }

        foreach (var errorLocation in updateError)
        {
            await this.setService.Update(errorLocation);
        }

        foreach (var currentElementSet in
                 currentModuleElementSets.Where(currentElementSet => currentElementSet != null))
        {
            await this.setService.Delete(currentElementSet ?? throw new InvalidOperationException());
        }

        await this.repository.Update(updatedElement);
    }

    public async Task<bool> HasPublishedModule(Module module)
    {
        var result = await this.repository.HasPublishedModule(module);
        return result;
    }

    public Task<Module> GetModuleFromElementLocation(ElementLocation location)
    {
        var module = this.repository.GetModuleFromElementLocation(location);

        return module;
    }

    public async Task UpdateLibraryFolder(Module module)
    {
        var currentModule = await this.repository.GetById(module.ModuleId, 1);
        currentModule.LibraryFolderId = module.LibraryFolderId;

        await this.repository.UpdateLibraryFolderId(currentModule);
    }

    public async Task UpdateLibraryFolderWithElements(Module module)
    {
        var currentModule = await this.repository.GetById(module.ModuleId, 1);
        currentModule.LibraryFolderId = module.LibraryFolderId;
        currentModule.ElementSets = await this.setService.GetByModule(currentModule);

        var updateTasks = new List<Task>();
        foreach (var elementSet in currentModule.ElementSets ?? new List<ElementSet>())
        {
            elementSet?.Elements?.AsParallel().ForAll(x => updateTasks.Add(this.updateElement(currentModule, x)));
        }

        await Task.WhenAll(updateTasks);

        await this.repository.UpdateLibraryFolderId(currentModule);
    }

    public async Task Delete(Module module)
    {
        try
        {
            var publishedModule = new PublishedModule
            {
                Id = module.ModuleId
            };
            await this.publishedModuleRepository.DeleteById(publishedModule);
        }
        catch (Exception ex)
        {
        }

        await this.setService.DeleteModuleSets(module);
        await this.repository.Delete(module);
    }

    public async Task UpdateLocationAttribute(ElementLocation location)
    {
        await this.setService.UpdateElementLocationAttribute(location);
    }

    private async Task loadElement(Element element)
    {
        if (element.TypeId == (int)ElementType.Image || element.TypeId == (int)ElementType.Pdf)
        {
            var elementImageJson = JObject.Parse(element.Content);
            var isLink = false;

            if (elementImageJson.ContainsKey("Link"))
            {
                isLink = elementImageJson["Link"].Value<bool>();
            }

            if (isLink || elementImageJson.ContainsKey("url"))
            {
                element.Content = elementImageJson["url"].Value<string>();
                return;
            }

            var key = elementImageJson["Key"].Value<string>();
            //element.Content = await this.fileService.GetFileAsync(key);
        }

        else if (element.TypeId != (int)ElementType.Text && element.TypeId != (int)ElementType.Video)
        {
            element.Content = string.Empty;
        }
    }

    private async Task updateElement(Module module, ElementLocation element)
    {
        var loadedElement = await this.elementRepository.GetById(element.ElementId);
        loadedElement.LibraryFolderId = module.LibraryFolderId;
        await this.elementRepository.UpdateLibraryFolder(loadedElement);
    }

    #endregion
}