using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Controller.OrganizationPackageController;

[TestFixture]
[ExcludeFromCodeCoverage]
public class CreateAndDeleteTests
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
        this.organizationPackageService = new OrganizationPackageService(this.organizationPackageRepository, this.packageService);
        this.organizationPackageController =
            new CAMCMSServer.Controller.OrganizationPackageController(this.organizationPackageService,
                this.userService);
    }

    [Test]
    public async Task UpdateAssociatedOrganization()
    {
        var packageAndOrganizations = new PackageAndOrganizations()
        {
            OrganizationIds = new List<int>()
            {
                1,6
            },
            PackageId = 1

        };

        var result = await this.organizationPackageController.UpdateAssociatedOrganizations(packageAndOrganizations, 1, "");

        Assert.IsInstanceOf<OkResult>(result);
    }
    [Test]
    public async Task UpdateAssociatedOrganizationInvalidUserId()
    {
        var packageAndOrganizations = new PackageAndOrganizations()
        {
            OrganizationIds = new List<int>()
            {
                1,6
            },
            PackageId = 1

        };

        var result = await this.organizationPackageController.UpdateAssociatedOrganizations(packageAndOrganizations, 0, "");

        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [Test]
    public async Task UpdateAssociatedPackages()
    {
        var organizationAndPackages = new OrganizationAndPackages()
        {
            OrganizationId = 1,
            PackageIds = new List<int>()
            {
                1, 2
            }
        };
        var result = await this.organizationPackageController.UpdateAssociatedPackages(organizationAndPackages, 1, "");

        Assert.IsInstanceOf<OkResult>(result);
    }

    [Test]
    public async Task UpdateAssociatedPackagesInvalidUserId()
    {
        var organizationAndPackages = new OrganizationAndPackages()
        {
            OrganizationId = 1,
            PackageIds = new List<int>()
            {
                1, 2
            }
        };
        var result = await this.organizationPackageController.UpdateAssociatedPackages(organizationAndPackages, 0, "");

        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }
    #endregion
}
