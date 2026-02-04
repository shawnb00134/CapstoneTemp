using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.OrganizationContentRoleRepository
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class DeleteTests
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
        public void DeleteOrganizationContentRole()
        {
            var organizationContentRoleId = 1;

            var countBefore = MockDataContext.OrganizationContentRoles.Count;

            this.organizationContentRoleRepository.DeleteOrganizationContentRole(organizationContentRoleId);

            var countAfter = MockDataContext.OrganizationContentRoles.Count;

            Assert.AreNotEqual(countBefore,countAfter);
        }


        #endregion
    }
}
