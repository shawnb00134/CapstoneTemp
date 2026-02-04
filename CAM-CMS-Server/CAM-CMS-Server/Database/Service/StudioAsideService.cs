using CAMCMSServer.Model;

namespace CAMCMSServer.Database.Service;

public interface IStudioAsideService
{
    #region Methods

    Task<StudioAside> GetStudioAside(int userId);

    #endregion
}

public class StudioAsideService : IStudioAsideService
{
    #region Data members

    private readonly IPackageService packageService;
    private readonly ILibraryFolderService libraryFolderService;
    private readonly IModuleService moduleService;
    private readonly IElementService elementService;

    #endregion

    #region Constructors

    #region Constructor

    public StudioAsideService(IPackageService packageService, ILibraryFolderService libraryFolderService,
        IModuleService moduleService, IElementService elementService)
    {
        this.packageService = packageService;
        this.libraryFolderService = libraryFolderService;
        this.moduleService = moduleService;
        this.elementService = elementService;
    }

    #endregion

    #endregion

    #region Methods

    public async Task<StudioAside> GetStudioAside(int userId)
    {
        var foldersTask = this.libraryFolderService.GetAuthorizedLibraryFolders(userId);
      
        var packagesTask = this.packageService.GetAuthorizedPackages(userId);

        await Task.WhenAll(foldersTask, packagesTask);

        var foldersResult = foldersTask.Result;
        var folders = new List<LibraryFolder>();

        if (foldersResult != null)
        {
            folders = foldersResult.ToList();
        }


        var packagesResult = packagesTask.Result;
        var packages = new List<Package>();

        if (packagesResult != null)
        {
            packages = packagesResult.ToList();
        }

        Parallel.ForEach(folders, folder =>
        {
            folder.Modules = new List<Module>();
            folder.Elements = new List<Element>();
        });
        var studioAside = new StudioAside
        {
            LibraryFolders = folders,
            Packages = packages
        };

        return studioAside;
    }

    #endregion
}