using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.OrganizationContentRoleService
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class DeleteTests
    {
        #region Data memebers

        private IOrganizationContentRoleRepository organizationContentRoleRepository;

        private IOrganizationContentRoleService organizationContentRoleService;

        #endregion

        #region Methods

        [SetUp]
        public void SetUp()
        {
            this.organizationContentRoleRepository = new OrganizationContentRoleRepository(new MockDataContext());

            this.organizationContentRoleService =
                new CAMCMSServer.Database.Service.OrganizationContentRoleService(
                    this.organizationContentRoleRepository);
        }

        [Test]
        public void DeleteOrganizationContentRole()
        {
            var organizationContentRoleId = 1;

            var countBefore = MockDataContext.OrganizationContentRoles.Count;

            this.organizationContentRoleService.DeleteOrganizationContentRole(organizationContentRoleId);

            var countAfter = MockDataContext.OrganizationContentRoles.Count;

            Assert.AreNotEqual(countBefore, countAfter);
        }



        #endregion
    }
}
