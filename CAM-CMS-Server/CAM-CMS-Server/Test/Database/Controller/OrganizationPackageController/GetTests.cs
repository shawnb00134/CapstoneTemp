using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Controller.OrganizationPackageController;

[TestFixture]
[ExcludeFromCodeCoverage]
public class GetTests
{
    #region Data members

    private IOrganizationPackageRepository organizationPackageRepository;

    private IOrganizationPackageService organizationPackageService;

    private IUserRepository userRepository;

    private IUserService userService;

    private IPackageRepository packageRepository;

    private IPackageService packageService;

    private IPackageFolderModuleRepository packageFolderModuleRepository;

    private CAMCMSServer.Controller.OrganizationPackageController organizationPackageController;

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
        this.organizationPackageRepository = new OrganizationPackageRepository(context);
        this.organizationPackageService =
            new OrganizationPackageService(this.organizationPackageRepository, this.packageService);
        this.organizationPackageController =
            new CAMCMSServer.Controller.OrganizationPackageController(this.organizationPackageService,
                this.userService);
    }

    [Test]
    public async Task GetOrganizationsByPackageId()
    {
        var packageId = 1;

        var orgs = this.organizationPackageController.GetOrganizationById(packageId, 1, "");

        Assert.IsInstanceOf<OkObjectResult>(orgs.Result);
    }

    [Test]
    public async Task GetPackageByOrganizationId()
    {
        var organizationId = 1;

        var packages = this.organizationPackageController.GetOrganizationById(organizationId, 1, "");

        Assert.IsInstanceOf<OkObjectResult>(packages.Result);
    }

    [Test]
    public async Task GetOrganizationsByPackageIdInvalidUserId()
    {
        var packageId = 1;

        var orgs = this.organizationPackageController.GetOrganizationById(packageId, 0, "");

        Assert.IsInstanceOf<BadRequestObjectResult>(orgs.Result);
    }

    [Test]
    public async Task GetPackageByOrganizationIdInvalid()
    {
        var organizationId = 1;

        var packages = this.organizationPackageController.GetOrganizationById(organizationId, 0, "");

        Assert.IsInstanceOf<BadRequestObjectResult>(packages.Result);
    }

    #endregion
}