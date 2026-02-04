using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Controller.StudioAsideController;

public class ConstructorTests
{
    #region Data members

    private IPackageService packageService;
    private ILibraryFolderService libraryFolderService;
    private IModuleService moduleService;
    private IElementService elementService;
    private IStudioAsideService studioAsideService;
    private IUserService userService;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();
        var packageRepo = new PackageRepository(context);
        var libaryFolder = new LibraryFolderRepository(context);
        var moduleRepo = new ModuleRepository(context);
        var elementRepo = new ElementRepository(context);
        var packageFolderModuleRepo = new PackageFolderModuleRepository(context);
        var publishModule = new PublishedModuleRepository(context);
        var elementSetRepo = new ElementSetRepository(context);
        //var fileService = new FileService(null, "nothing");
        //var elementSetService = new ElementSetService(elementSetRepo, this.elementService, fileService);

        this.libraryFolderService = new LibraryFolderService(libaryFolder);
        this.packageService = new PackageService(packageRepo, packageFolderModuleRepo);
        this.elementService = new ElementService(elementRepo);
        //this.moduleService = new ModuleService(moduleRepo, elementSetService, this.elementService, publishModule, null);
        this.studioAsideService = new StudioAsideService(this.packageService, this.libraryFolderService,
            this.moduleService, this.elementService);
        this.userService = new UserService(new UserRepository(context), new InvitationRepository(context));
    }

    [Test]
    public void NotNullTest()
    {
        var studioAsideController = new CAMCMSServer.Controller.StudioAsideController(this.studioAsideService, this.userService);
        Assert.IsNotNull(studioAsideController);
    }

    #endregion
}