using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.OrganizationContentRoleService
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class GetTests
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
        public void GetOrganizationContentRoles()
        {
            var organizationId = 1;

            var result =
                this.organizationContentRoleService.GetOrganizationContentRolesForOrganization(organizationId).Result;
            var actual = MockDataContext.OrganizationContentRoles.Where(x => x.OrganizationId == organizationId);


            Assert.IsNotNull(result);
            Assert.AreEqual(actual.Count(), result.Count());

        }


        #endregion
    }
}
