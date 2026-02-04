using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Controller.OrganizationController;

public class CreateTests
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
    public void CreateOneValidOrganization()
    {
        var organization = new Organization
        {
            CreatedAt = null,
            IsActive = true,
            Name = "Test Organization",
            OrganizationId = 1,
            Tags = new[] { "Test", "Organization" },
            UpdatedAt = null
        };
        Assert.IsInstanceOf<OkObjectResult>(this.organizationController.CreateOrganization(organization, 1, "").Result);
    }

    [Test]
    public void CreateNullOrganization()
    {
        var organization = new Organization
        {
            CreatedAt = null,
            IsActive = true,
            Name = "Test Organization",
            OrganizationId = 1,
            Tags = new[] { "Test", "Organization" },
            UpdatedAt = null
        };
        Assert.IsInstanceOf<BadRequestObjectResult>(this.organizationController.CreateOrganization(null, 1, "").Result);
    }

    [Test]
    public void CreateValidOrganizationInvalidUserId()
    {
        var organization = new Organization
        {
            CreatedAt = null,
            IsActive = true,
            Name = "Test Organization",
            OrganizationId = 1,
            Tags = new[] { "Test", "Organization" },
            UpdatedAt = null
        };
        Assert.IsInstanceOf<BadRequestObjectResult>(this.organizationController.CreateOrganization(organization, 0, "")
            .Result);
    }

    #endregion
}