using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Controller.OrganizationContentRoleController
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ConstructorTests
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

        [Test]
        public void TestNotNull()
        {
            this.organizationContentRoleRepository = new OrganizationContentRoleRepository(new MockDataContext());
            this.userRepository = new UserRepository(new MockDataContext());
            this.userService = new UserService(this.userRepository, this.invitationRepository);
            this.organizationContentRoleService =
                new OrganizationContentRoleService(this.organizationContentRoleRepository);

            this.controller =
                new CAMCMSServer.Controller.OrganizationContentRoleController(this.userService,
                    this.organizationContentRoleService);

            Assert.NotNull(this.controller);
        }


        #endregion
    }
}
