using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.OrganizationContentRoleService
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ConstructorTests
    {
        #region Data memebers

        private IOrganizationContentRoleRepository organizationContentRoleRepository;

        private IOrganizationContentRoleService organizationContentRoleService;

        #endregion

        [Test]
        public void NotNullTest()
        {
            this.organizationContentRoleRepository = new OrganizationContentRoleRepository(new MockDataContext());

            this.organizationContentRoleService =
                new CAMCMSServer.Database.Service.OrganizationContentRoleService(
                    this.organizationContentRoleRepository);

            Assert.NotNull(this.organizationContentRoleService);
        }
    }
}
