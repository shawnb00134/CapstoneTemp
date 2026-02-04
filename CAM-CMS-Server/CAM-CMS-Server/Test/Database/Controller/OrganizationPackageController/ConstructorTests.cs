using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Controller.OrganizationPackageController;

[TestFixture]
[ExcludeFromCodeCoverage]
public class ConstructorTests
{
    #region Data members

    private IOrganizationPackageRepository organizationPackageRepository;

    private IOrganizationPackageService organizationPackageService;

    private IUserRepository userRepository;

    private IUserService userService;

    private IPackageRepository packageRepository;

    private IPackageService packageService;

    private IPackageFolderModuleRepository packageFolderModuleRepository;

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
    }

    [Test]
    public void NotNullTest()
    {
        var organizationController =
            new CAMCMSServer.Controller.OrganizationPackageController(this.organizationPackageService,
                this.userService);

        Assert.IsNotNull(organizationController);
    }

    #endregion
}