using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Controller.PackageController;

[TestFixture]
[ExcludeFromCodeCoverage]
public class ConstructorTests
{
    #region Data members

    private IPackageRepository packageRepository;

    private IPackageService packageService;

    private IUserRepository userRepository;

    private IPackageFolderModuleRepository packageFolderModuleRepository;

    private IUserService userService;

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
    }

    [Test]
    public void NotNullTest()
    {
        var packageController = new CAMCMSServer.Controller.PackageController(this.packageService, this.userService);

        Assert.IsNotNull(packageController);
    }

    #endregion
}