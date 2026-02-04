using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Model.ElementTypes;
using CAMCMSServer.Model.Requests;
using Newtonsoft.Json.Linq;
using Npgsql;

namespace CAMCMSServer.Database.Service;

public interface IElementSetService
{
    #region Methods

    Task<ElementSet> GetById(int id);
    Task<IEnumerable<ElementSet>> GetByModule(Module module);
    Task Create(ElementSet module);
    Task AddElement(ElementLocation module);
    Task Update(ElementSet updatedElement);
    Task UpdateSet(ElementSet updatedElement);
    Task UpdateLocation(ElementLocation updatedLocation);
    Task UpdateLocationElement(ElementLocation updatedLocation);
    Task UpdateLocationSet(ElementLocation updatedLocation, int newSetId);
    Task UpdateSetOrder(ElementSet updatedSet);
    Task UpdateLocationOrder(ElementLocation updatedLocation);
    Task Delete(ElementSet module);
    Task DeleteElement(ElementLocation module);
    Task DeleteModuleSets(Module module);
    Task UpdateElementLocationAttribute(ElementLocation location);

    #endregion

    Task<int> GetLibraryFolderIdFromModuleId(int? newSetModuleId);
    Task<int> GetParentModuleLibraryFolderIdFromElementLocation(ElementLocation newElement);
    Task<int> GetLibraryFolderIdFromElementSet(ElementSet elementSet);
}

public class ElementSetService : IElementSetService
{
    #region Data members

    private readonly IElementSetRepository repository;
    private readonly IElementService elementRepository;
    //private readonly IFileService fileService;

    #endregion

    #region Constructors

    public ElementSetService(IElementSetRepository repository, IElementService elementRepository)
    {
        this.repository = repository;
        this.elementRepository = elementRepository;
        //this.fileService = fileService;
    }

    #endregion

    #region Methods

    public async Task<ElementSet> GetById(int id)
    {
        var set = await this.repository.GetById(id, 1);

        await this.loadElementsInSet(set);

        return set;
    }

    public async Task<IEnumerable<ElementSet>> GetByModule(Module module)
    {
        var sets = await this.repository.GetAll(module);

        var elementSets = sets as ElementSet[] ?? sets.ToArray();

        //var setTasks = new List<Task>();
        //elementSets.AsParallel().ForAll(x => setTasks.Add(this.loadElementsInSet(x))); TODO: Parallel not working?

        foreach (var elementSet in elementSets)
        {
            await this.loadElementsInSet(elementSet);
        }

        //await Task.WhenAll(setTasks);

        return elementSets;
    }

    public async Task Create(ElementSet module)
    {
        if (module == null)
        {
            throw new ArgumentNullException(nameof(module));
        }

        if (module.Styling == null)
        {
            throw new ArgumentNullException(nameof(module.Styling));
        }

        module.IsEditable ??= false;

        module.Styling.is_appendix ??= false;

        await this.repository.Create(module);
    }

    public async Task AddElement(ElementLocation newElement)
    {
        if (newElement.Element is { TypeId: 7 })
        {
            var fetchedElement = this.elementRepository.GetById(newElement.ElementId).Result;
            newElement.Element.Content = fetchedElement.Content;
        }

        await this.repository.CreateLocation(newElement);
    }

    public async Task Delete(ElementSet module)
    {
        await this.repository.Delete(module);
    }

    public async Task DeleteElement(ElementLocation deleteLocation)
    {
        await this.repository.DeleteLocation(deleteLocation);
    }

    public async Task Update(ElementSet updatedElement)
    {
        if (updatedElement.SetLocationId == null)
        {
            throw new ArgumentNullException(nameof(updatedElement.SetLocationId));
        }

        updatedElement.Elements ??= new List<ElementLocation>();

        var currentSet = await this.repository.GetById((int)updatedElement.SetLocationId, 1);

        currentSet.Elements = await this.repository.GetSetElements(currentSet);

        var currentSetLocations = currentSet.Elements.ToList();

        var updateError = new List<ElementLocation>();
        foreach (var updatedLocation in updatedElement.Elements)
        {
            if (updatedLocation == null)
            {
                continue;
            }

            var found = currentSetLocations.Find(x => x != null && x.Equals(updatedLocation));
            if (found != null)
            {
                try
                {
                    await this.repository.UpdateLocation(updatedLocation);
                }
                catch (PostgresException)
                {
                    updateError.Add(updatedLocation);
                    var tempLocation = new ElementLocation
                    {
                        ElementId = updatedLocation.ElementId,
                        SetLocationId = updatedLocation.SetLocationId,
                        Place = int.MaxValue,
                        IsEditable = updatedLocation.IsEditable,
                        Attributes = new SetLocationAttributes
                        {
                            Width = updatedLocation.Attributes?.Width,
                            Height = updatedLocation.Attributes?.Height,
                            Alignment = updatedLocation.Attributes?.Alignment,
                            HeadingLevel = Convert.ToInt32(updatedLocation.Attributes?.HeadingLevel),
                            DisplayLink = Convert.ToBoolean(updatedLocation.Attributes?.DisplayLink)
                        }
                    };
                    await this.repository.UpdateLocation(tempLocation);
                }

                currentSetLocations.Remove(found);
            }
            else
            {
                await this.repository.CreateLocation(updatedLocation);
            }
        }

        foreach (var errorLocation in updateError)
        {
            await this.repository.UpdateLocation(errorLocation);
        }

        foreach (var currentElementSet in currentSetLocations.Where(currentElementSet => currentElementSet != null))
        {
            await this.repository.DeleteLocation(currentElementSet ?? throw new InvalidOperationException());
        }

        await this.repository.Update(updatedElement);
    }

    public async Task UpdateSet(ElementSet updatedSet)
    {
        await this.repository.Update(updatedSet);
    }

    public async Task UpdateLocation(ElementLocation updatedLocation)
    {
        await this.repository.UpdateLocation(updatedLocation);
    }

    public async Task UpdateLocationElement(ElementLocation updatedLocation)
    {
        await this.repository.UpdateLocationElement(updatedLocation);
    }

    public async Task UpdateLocationSet(ElementLocation updatedLocation, int newSetId)
    {
        var elementSet = new ElementSet
        {
            SetLocationId = newSetId
        };

        elementSet.Elements = await this.repository.GetSetElements(elementSet);

        updatedLocation.Place = elementSet.Elements.Count();

        await this.repository.UpdateLocationSet(updatedLocation, newSetId);
    }

    public async Task UpdateSetOrder(ElementSet updatedSet)
    {
        await this.repository.UpdateSetOrder(updatedSet);
    }

    public async Task UpdateLocationOrder(ElementLocation updatedLocation)
    {
        await this.repository.UpdateLocationOrder(updatedLocation);
    }

    public async Task DeleteModuleSets(Module module)
    {
        var sets = await this.GetByModule(module);

        var deleteTasks = new List<Task>();
        sets.AsParallel().ForAll(x => deleteTasks.Add(this.Delete(x)));

        await Task.WhenAll(deleteTasks);
    }

    public async Task UpdateElementLocationAttribute(ElementLocation location)
    {
        var updatedLocation = new ElementLocation
        {
            ElementId = location.ElementId,
            SetLocationId = location.SetLocationId,
            IsEditable = location.IsEditable,
            Attributes = new SetLocationAttributes
            {
                Width = location.Attributes?.Width,
                Height = location.Attributes?.Height,
                Alignment = location.Attributes?.Alignment,
                HeadingLevel = Convert.ToInt32(location.Attributes?.HeadingLevel),
                DisplayLink = Convert.ToBoolean(location.Attributes?.DisplayLink)
            }
        };
        await this.repository.UpdateElementLocationAttributes(updatedLocation);
    }

    public async Task<int> GetLibraryFolderIdFromModuleId(int? newSetModuleId)
    {
        return await this.repository.GetLibraryFolderIdFromModuleId(newSetModuleId);
    }

    public async Task<int> GetParentModuleLibraryFolderIdFromElementLocation(ElementLocation newElement)
    {
        return await this.repository.GetParentModuleLibraryFolderIdFromElementLocation(newElement);
    }

    public async Task<int> GetLibraryFolderIdFromElementSet(ElementSet elementSet)
    {
        return await this.repository.GetLibraryFolderIdFromElementSet(elementSet);
    }

    private async Task loadElementsInSet(ElementSet set)
    {
        set.Elements = await this.repository.GetSetElements(set);

        //var elementTasks = new List<Task>();
        //set.Elements.AsParallel()
        //    .ForAll(x => elementTasks.Add(this.loadElement(x ?? throw new InvalidOperationException())));

        //await Task.WhenAll(elementTasks);

        foreach (var location in set.Elements)
        {
            location.Attributes ??= new SetLocationAttributes();

            await this.loadElement(location);
        }
    }

    /// <summary>
    ///     Loads an element from the database with the given location and returns it with the element's content.
    ///     If the element contains the key "Link" and the value is true, the element's content will be the url of the link.
    ///     else the element's content will be the file content from the file service.
    /// </summary>
    /// <param name="location">The location.</param>
    private async Task loadElement(ElementLocation location)
    {
        location.Element = await this.elementRepository.GetById(location.ElementId);
        if (location.Element.TypeId == (int)ElementType.Image || location.Element.TypeId == (int)ElementType.Pdf)
        {
            var elementImageJson = JObject.Parse(location.Element.Content);
            var isLink = false;

            if (elementImageJson.ContainsKey("Link"))
            {
                isLink = elementImageJson["Link"].Value<bool>();
            }

            if (isLink || elementImageJson.ContainsKey("url"))
            {
                location.Element.Content = elementImageJson["url"].Value<string>();
                return;
            }

            var key = elementImageJson["Key"].Value<string>();
            //location.Element.Content = await this.fileService.GetFileAsync(key);
        }

        else if (location.Element.TypeId != (int)ElementType.Text && location.Element.TypeId != (int)ElementType.Video)
        {
            location.Element.Content = string.Empty;
        }
    }

    #endregion
}