using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Controller.OrganizationContentRoleController
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class DeleteTests
    {
        #region Data members

        private IOrganizationContentRoleRepository organizationContentRoleRepository;

        private IOrganizationContentRoleService organizationContentRoleService;

        private IUserRepository userRepository;

        private IUserService userService;

        private IInvitationRepository invitationRepository;

        private CAMCMSServer.Controller.OrganizationContentRoleController controller;

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
        public void DeleteOrganizationContentRole()
        {
            var organizationContentRoleId = 1;

            var validUser = 1;

            Assert.IsInstanceOf<OkObjectResult>(this.controller.DeleteOrganizationContentRole(organizationContentRoleId,validUser, "").Result);
        }

        [Test]
        public void DeleteOrganizationContentRoleInvalidUser()
        {
            var organizationContentRole = 1;

            var invalidUser = 0;

            Assert.IsInstanceOf<BadRequestObjectResult>(
                this.controller.DeleteOrganizationContentRole(organizationContentRole, invalidUser, "").Result);
        }
        [Test]
        public void DeleteOrganizationContentRoleInvalidContentRoleId()
        {
            var organizationContentRole = 0;

            var validUser = 1;

            Assert.IsInstanceOf<BadRequestObjectResult>(
                this.controller.DeleteOrganizationContentRole(organizationContentRole, validUser, "").Result);
        }

        #endregion
    }
}
