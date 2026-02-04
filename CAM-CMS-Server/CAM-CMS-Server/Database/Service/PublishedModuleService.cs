using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CAMCMSServer.Database.Service;

public interface IPublishedModuleService
{
    #region Methods

    Task Create(PublishedModule module);

    Task<IEnumerable<PublishedModule>> GetAll();

    Task<PublishedModule> GetById(int id);

    Task Update(PublishedModule module);

    Task Delete(PublishedModule module);

    Task DeleteById(PublishedModule module);

    #endregion

    Task<int> GetLibraryFolderIdFromModuleId(int publishedModuleId);
}

/// <summary>
///     PublishedModuleService
/// </summary>
/// <seealso cref="IPublishedModuleService" />
public class PublishedModuleService : IPublishedModuleService
{
    #region Data members

    private readonly IPublishedModuleRepository repository;
    private readonly IElementRepository elementRepository;
    //private readonly IFileService fileService;
    private readonly ILogger<PublishedModuleService> logger;

    #endregion

    #region Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="PublishedModuleService" /> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="elementRepository">The element repository.</param>
    /// <param name="fileService">The file service.</param>
    /// <param name="logger">The logger.</param>
    public PublishedModuleService(IPublishedModuleRepository repository, IElementRepository elementRepository, ILogger<PublishedModuleService> logger)
    {
        this.repository = repository;
        this.elementRepository = elementRepository;
        //this.fileService = fileService;
        this.logger = logger;
    }

    #endregion

    #region Methods

    /// <summary>
    ///     Creates the specified module.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <exception cref="System.ArgumentNullException">module</exception>
    public async Task Create(PublishedModule module)
    {
        if (module == null)
        {
            throw new ArgumentNullException(nameof(module));
        }

        var publishedModule = await this.SwapLinkForKey(module);
        await this.repository.Create(publishedModule);
    }

    /// <summary>
    ///     Deletes the by identifier.
    /// </summary>
    /// <param name="module">The module.</param>
    public async Task DeleteById(PublishedModule module)
    {
        await this.repository.DeleteById(module);
    }

    public async Task<int> GetLibraryFolderIdFromModuleId(int publishedModuleId)
    {
        return await this.repository.GetLibraryFolderIdFromModuleId(publishedModuleId);
    }

    /// <summary>
    ///     Updates the specified module.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <exception cref="System.ArgumentNullException">module</exception>
    public async Task Update(PublishedModule module)
    {
        if (module == null)
        {
            throw new ArgumentNullException(nameof(module));
        }

        var publishedModule = await this.SwapLinkForKey(module);
        await this.repository.Update(publishedModule);
    }

    /// <summary>
    ///     Gets all.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<PublishedModule>> GetAll()
    {
        var modules = await this.repository.GetAll();
        var validModules = new List<PublishedModule>();

        foreach (var publishedModule in modules)
        {
            try
            {
                await this.swapKeyForLink(publishedModule);
                validModules.Add(publishedModule);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error processing module with ID {publishedModule.Id}: {ex.Message}");
            }
        }

        return validModules.OrderBy(module => module.Id);
    }

    /// <summary>
    ///     Gets the by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    public async Task<PublishedModule> GetById(int id)
    {
        var fetchedModule = this.repository.GetById(id, 1).Result;
        if (fetchedModule == null)
        {
            return null;
        }

        var readyModule = await this.swapKeyForLink(fetchedModule);
        return readyModule;
    }

    Task IPublishedModuleService.Delete(PublishedModule module)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Swaps element's key for a link in the element's content to be displayed on the front end with a updated link and
    ///     not show the file's key to the frontend
    /// </summary>
    /// <param name="module">The module.</param>
    /// //TODO: Add functionality for video files
    /// <returns></returns>
    private async Task<PublishedModule> swapKeyForLink(PublishedModule module)
    {
        var cacheJson = JObject.Parse(module.Cache);
        var pages = cacheJson["pages"];

        foreach (var page in pages)
        {
            var locations = page[0]["locations"];

            foreach (var location in locations)
            {
                var elementJson = location["element"];
                var elementJsonId = elementJson["elementId"].Value<int>();
                var elementTypeId = elementJson["typeId"].Value<int>();

                if (elementTypeId is 2 or 5)
                {
                    var contentJson = elementJson["content"].Value<string>();
                    if (!contentJson.Contains("http"))
                    {
                        //var url = await this.fileService.GetFileAsync(contentJson);
                        //elementJson["content"] = url;
                    }
                }
            }
        }

        module.Cache = JsonConvert.SerializeObject(cacheJson, Formatting.Indented);
        return module;
    }

    /// <summary>
    ///     Swaps element's link for key in the element's content.
    /// </summary>
    /// <param name="module">The module.</param>
    /// //TODO: Add functionality for video files
    /// <returns></returns>
    private async Task<PublishedModule> SwapLinkForKey(PublishedModule module)
    {
        var cacheJson = JObject.Parse(module.Cache);
        var elementSets = cacheJson["elementSets"];

        foreach (var currentElementSet in elementSets)
        {
            var locations = currentElementSet["locations"];

            foreach (var location in locations)
            {
                var elementJson = location["element"];
                var elementJsonId = elementJson["elementId"].Value<int>();
                var fetchedElement = await this.elementRepository.GetById(elementJsonId, 1);
                if (fetchedElement.TypeId is 2 or 5)
                {
                    var fetchedContent = JObject.Parse(fetchedElement.Content);
                    try
                    {
                        var key = fetchedContent["Key"].Value<string>();
                        elementJson["content"] = key;
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, $"Error processing element with ID {elementJsonId} in module {module.Id}: {e.Message}");
                    }
                }
            }
        }

        module.Cache = JsonConvert.SerializeObject(cacheJson, Formatting.Indented);
        return module;
    }

    #endregion
}
