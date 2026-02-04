using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.OrganizationContentRoleService
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class CreateTests
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
            this.organizationContentRoleService.CreateOrganizationContentRole(newOrganizationContentRole);
            var result = MockDataContext.OrganizationContentRoles.Where(x =>
                x.OrganizationContentRoleId == newOrganizationContentRole.OrganizationContentRoleId);
            var countAfter = MockDataContext.OrganizationContentRoles.Count;
            Assert.AreNotEqual(countBefore, countAfter);
            Assert.NotNull(result.ElementAt(0));
            
        }
        #endregion
    }
}
