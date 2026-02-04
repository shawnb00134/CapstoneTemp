using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.OrganizationContentRoleRepository
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class CreateTests
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
        public void CreateOrganizationContentRole()
        {
            var newOrganizationContentRole = new OrganizationContentRole
            {
                ContentRoleId = 2,
                CreatedAt = ",",
                CreatedBy = 1,
                OrganizationId = 1,
                UpdatedAt = ",",
                UpdatedBy = 1,
                OrganizationContentRoleId = 3
            };
            var countBefore = MockDataContext.OrganizationContentRoles.Count;
            this.organizationContentRoleRepository.CreateOrganizationContentRole(newOrganizationContentRole);
            var result = MockDataContext.OrganizationContentRoles.Where(x =>
                x.OrganizationContentRoleId == newOrganizationContentRole.OrganizationContentRoleId);
            var countAfter = MockDataContext.OrganizationContentRoles.Count;
            Assert.AreNotEqual(countBefore,countAfter);
            Assert.NotNull(result.ElementAt(0));
        }

        #endregion
    }
}
