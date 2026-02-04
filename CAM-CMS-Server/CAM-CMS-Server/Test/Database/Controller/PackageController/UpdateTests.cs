using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Controller.PackageController;

public class UpdateTests
{
    #region Data members

    private IPackageRepository packageRepository;

    private IPackageService packageService;

    private IUserRepository userRepository;

    private IPackageFolderModuleRepository packageFolderModuleRepository;

    private IUserService userService;

    private CAMCMSServer.Controller.PackageController packageController;

    private IInvitationRepository invitationRepository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();
        this.userRepository = new UserRepository(context);
        this.packageRepository = new PackageRepository(context);
        this.packageFolderModuleRepository = new PackageFolderModuleRepository(context);
        this.packageService = new PackageService(this.packageRepository, this.packageFolderModuleRepository);
        this.userService = new UserService(this.userRepository, this.invitationRepository);

        this.packageController = new CAMCMSServer.Controller.PackageController(this.packageService, this.userService);
    }

    [Test]
    public void UpdatePackageUnsupported()
    {
        var userId = 1;
        var package = new Package
        {
            CreatedAt = null,
            CreatedBy = 1,
            IsCore = true,
            Name = "Package 1",
            PackageFolders = new List<PackageFolder>(),
            PackageId = 1,
            PackageTypeId = 1,
            PublishedAt = null,
            UpdatedAt = null,
            UpdatedBy = null
        };
        var result = this.packageController.UpdatePackage(package, userId, "").Result;
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [Test]
    public void UpdatePackageUnsupportedInvalidUserId()
    {
        var userId = 0;
        var package = new Package
        {
            CreatedAt = null,
            CreatedBy = 1,
            IsCore = true,
            Name = "Package 1",
            PackageFolders = new List<PackageFolder>(),
            PackageId = 1,
            PackageTypeId = 1,
            PublishedAt = null,
            UpdatedAt = null,
            UpdatedBy = null
        };
        var result = this.packageController.UpdatePackage(package, userId, "").Result;
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [Test]
    public void UpdatePackageFolderValid()
    {
        var userId = 1;

        var packageFolder = new PackageFolder
        {
            ContentRoleId = 1,
            CreatedBy = 2,
            CreatedAt = null,
            DisplayName = "New Name",
            Editable = true,
            FolderTypeId = 1,
            FullDescription = "packageFolder1",
            OrderInParent = 0,
            PackageFolderId = 1,
            PackageFolders = new List<PackageFolder?>(),
            PackageId = 1,
            PackageFoldersModule = new List<PackageFolderModule>(),
            ShortDescription = "package f 1",
            PublishedAt = null,
            Published = false,
            ParentFolderId = null,
            UpdatedAt = null,
            UpdatedBy = null,
            Thumbnail = null
        };
        var result = this.packageController.UpdatePackageFolder(packageFolder, userId, "").Result;
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<OkObjectResult>(result);
    }

    [Test]
    public void UpdatePackageFolderInvalidUserId()
    {
        var userId = 0;

        var packageFolder = new PackageFolder
        {
            ContentRoleId = 1,
            CreatedBy = 2,
            CreatedAt = null,
            DisplayName = "New Name",
            Editable = true,
            FolderTypeId = 1,
            FullDescription = "packageFolder1",
            OrderInParent = 0,
            PackageFolderId = 1,
            PackageFolders = new List<PackageFolder?>(),
            PackageId = 1,
            PackageFoldersModule = new List<PackageFolderModule>(),
            ShortDescription = "package f 1",
            PublishedAt = null,
            Published = false,
            ParentFolderId = null,
            UpdatedAt = null,
            UpdatedBy = null,
            Thumbnail = null
        };
        var result = this.packageController.UpdatePackageFolder(packageFolder, userId, "").Result;

        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [Test]
    public void UpdatePackageFolderInvalidPackageFolder()
    {
        var userId = 1;
        var result = this.packageController.UpdatePackageFolder(null, userId, "").Result;

        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    #endregion
}