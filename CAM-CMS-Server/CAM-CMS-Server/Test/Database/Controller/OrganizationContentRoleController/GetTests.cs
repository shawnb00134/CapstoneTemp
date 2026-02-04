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
    public class GetTests
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
        public void GetOrganizationContentRoles()
        {
            var organizationId = 1;

            var validUser = 1;

            Assert.NotNull(this.controller.GetContentRoles(organizationId,validUser, "").Result);
        }

        [Test]
        public void GetOrganizationContentRolesInvalidUser()
        {
            var organizationId = 1;

            var invalidUser = 0;

            Assert.IsInstanceOf<BadRequestObjectResult>(this.controller.GetContentRoles(organizationId, invalidUser, "").Result);
        }

        [Test]
        public void GetOrganizationContentRolesInvalidOrganizationId()
        {
            var organizationId = 0;

            var validUser = 1;

            Assert.IsInstanceOf<BadRequestObjectResult>(this.controller.GetContentRoles(organizationId, validUser, "").Result);
        }


        #endregion
    }
}
