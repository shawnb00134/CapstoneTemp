using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.OrganizationContentRoleRepository
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class GetTests
    {

        #region Data members
        private IOrganizationContentRoleRepository organizationContentRoleRepository;

        #endregion


        #region Methods

        [SetUp]
        public void SetUp()
        {
            this.organizationContentRoleRepository =
                new CAMCMSServer.Database.Repository.OrganizationContentRoleRepository(new MockDataContext());
        }

        [Test]
        public void GetOrganizationContentRoles()
        {
            var organizationId = 1;

            var result =
                this.organizationContentRoleRepository.GetOrganizationContentRolesForOrganization(organizationId).Result;
            var actual = MockDataContext.OrganizationContentRoles.Where(x => x.OrganizationId == organizationId);


            Assert.IsNotNull(result);
            Assert.AreEqual(actual.Count(), result.Count());

        }
        #endregion

    }
}
