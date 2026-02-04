using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.StudioAsideService;

public class GetAsideTests
{
    #region Data members

    private IPackageService packageService;
    private ILibraryFolderService libraryFolderService;
    private IModuleService moduleService;
    private IElementService elementService;
    private IStudioAsideService studioAsideService;

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
        var elementSetService = new CAMCMSServer.Database.Service.ElementSetService(elementSetRepo, this.elementService);

        this.libraryFolderService = new CAMCMSServer.Database.Service.LibraryFolderService(libaryFolder);
        this.packageService = new CAMCMSServer.Database.Service.PackageService(packageRepo, packageFolderModuleRepo);
        this.elementService = new CAMCMSServer.Database.Service.ElementService(elementRepo);
        this.moduleService = new CAMCMSServer.Database.Service.ModuleService(moduleRepo, elementSetService, this.elementService, publishModule);
        this.studioAsideService = new CAMCMSServer.Database.Service.StudioAsideService(this.packageService,
            this.libraryFolderService, this.moduleService, this.elementService);
    }

    [Test]
    public void GetAsideItems()
    {
        var items = this.studioAsideService.GetStudioAside(1).Result;

        Assert.IsNotNull(items);
        Assert.IsNotNull(items.LibraryFolders);
        Assert.IsNotEmpty(items.LibraryFolders);
        Assert.IsNotNull(items.Packages);
        Assert.IsNotEmpty(items.Packages);
    }

    #endregion
}