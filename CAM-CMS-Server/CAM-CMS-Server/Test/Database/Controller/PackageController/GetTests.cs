using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Controller.PackageController;

[TestFixture]
[ExcludeFromCodeCoverage]
public class GetTests
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
    public void GetAllPackagesValid()
    {
        var userId = 1;
        var result = this.packageController.GetAll(userId, "");
        Assert.IsNotNull(result);
    }

    [Test]
    public void GetAllPackagesInvalidUserId()
    {
        var userId = 0;

        var result = this.packageController.GetAll(userId, "").Result;

        Assert.IsInstanceOf<BadRequestObjectResult>(result);
        var badRequestResult = result as BadRequestObjectResult;
        Assert.AreEqual(userId, badRequestResult.Value);
    }

    [Test]
    public void GetPackageByIdValidId()
    {
        var userId = 1;
        var packageId = 1;
        var result = this.packageController.GetPackageById(packageId, userId, "").Result;
        Assert.IsNotNull(result);
    }

    [Test]
    public void GetPackageByIdInvalidId()
    {
        var userId = 0;
        var packageId = 1;

        var result = this.packageController.GetPackageById(packageId, userId, "").Result;

        Assert.IsInstanceOf<BadRequestObjectResult>(result);

        var badRequestResult = result as BadRequestObjectResult;

        Assert.AreEqual(userId, badRequestResult.Value);
    }

    #endregion
}