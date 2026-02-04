using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Controller.OrganizationContentRoleController
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class CreateTests
    {

        #region Data members

        private IOrganizationContentRoleRepository organizationContentRoleRepository;

        private IOrganizationContentRoleService organizationContentRoleService;

        private IUserRepository userRepository;

        private IUserService userService;

        private CAMCMSServer.Controller.OrganizationContentRoleController controller;

        private IInvitationRepository invitationRepository;

        #endregion

        #region Methods

        [SetUp]
        public void SetUp()
        {
            this.organizationContentRoleRepository = new OrganizationContentRoleRepository(new MockDataContext());
            this.userRepository = new UserRepository(new MockDataContext());
            this.userService = new UserService(this.userRepository, this.invitationRepository);
            this.organizationContentRoleService =
                new OrganizationContentRoleService(this.organizationContentRoleRepository);

            this.controller =
                new CAMCMSServer.Controller.OrganizationContentRoleController(this.userService,
                    this.organizationContentRoleService);
        }

        [Test]
        public void CreateOrganizationContentRole()
        {
            var organizationContentRole = new OrganizationContentRole()
            {
                ContentRoleId = 1,
                CreatedAt = "",
                CreatedBy = 1,
                DisplayName = "Test 1",
                OrganizationContentRoleId = 1,
                OrganizationId = 1,
                UpdatedAt = "",
                UpdatedBy = null

            };
            var validUserId = 1;

            Assert.IsInstanceOf<OkObjectResult>(this.controller.CreateOrganization(organizationContentRole, validUserId, "").Result);

        }

        [Test]
        public void CreateOrganizationContentRoleInvalidUser()
        {
            var organizationContentRole = new OrganizationContentRole()
            {
                ContentRoleId = 1,
                CreatedAt = "",
                CreatedBy = 1,
                DisplayName = "Test 1",
                OrganizationContentRoleId = 1,
                OrganizationId = 1,
                UpdatedAt = "",
                UpdatedBy = null

            };
            var invalidUserId = 0;

            Assert.IsInstanceOf<BadRequestObjectResult>(this.controller.CreateOrganization(organizationContentRole, invalidUserId, "").Result);
        }



        #endregion
    }
}
