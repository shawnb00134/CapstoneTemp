using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Controller.OrganizationController;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ConstructorTests
{
    #region Data members

    private IOrganizationRepository organizationRepository;
    private IOrganizationService organizationService;
    private IUserRepository userRepository;
    private IUserService userService;
    private IInvitationRepository invitationRepository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();
        this.organizationRepository = new OrganizationRepository(context);
        this.organizationService = new OrganizationService(this.organizationRepository);
        this.userRepository = new UserRepository(context);
        this.userService = new UserService(this.userRepository, this.invitationRepository);
    }

    [Test]
    public void NotNullTest()
    {
        var organizationController =
            new CAMCMSServer.Controller.OrganizationController(this.organizationService, this.userService);

        Assert.IsNotNull(organizationController);
    }

    #endregion
}