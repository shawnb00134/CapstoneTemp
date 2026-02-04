using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Controller.OrganizationController;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetTests
{
    #region Data members

    private IOrganizationRepository organizationRepository;
    private IOrganizationService organizationService;
    private IUserRepository userRepository;
    private IUserService userService;
    private CAMCMSServer.Controller.OrganizationController organizationController;
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
        this.organizationController =
            new CAMCMSServer.Controller.OrganizationController(this.organizationService, this.userService);
    }

    [Test]
    public void GetOneValidOrganizationById()
    {
        var organizationId = 1;
        Assert.IsNotNull(this.organizationController.GetOrganizationById(organizationId, 1, "").Result);
    }

    [Test]
    public void GetOneInvalidOrganizationById()
    {
        var organizationId = 0;
        Assert.IsInstanceOf<BadRequestResult>(this.organizationController.GetOrganizationById(organizationId, 1, "")
            .Result);
    }

    [Test]
    public void GetOrganizationByIdInvalidUserId()
    {
        var organizationId = 1;
        Assert.IsInstanceOf<BadRequestObjectResult>(this.organizationController.GetOrganizationById(organizationId, 0, "")
            .Result);
    }

    [Test]
    public void GetAllOrganizations()
    {
        Assert.IsNotNull(this.organizationController.GetAllAuthorizedOrganizations(1, "").Result);
    }

    [Test]
    public void GetAllOrganizationsInvalidUserId()
    {
        Assert.IsInstanceOf<BadRequestObjectResult>(this.organizationController.GetAllAuthorizedOrganizations(0, "").Result);
    }

    #endregion
}