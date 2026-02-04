using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.OrganizationContentRoleRepository
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ConstructorTests
    {
        #region Data members
        private IOrganizationContentRoleRepository organizationContentRoleRepository;
        #endregion
        #region Methods

        [SetUp]
        public void SetUp()
        {
            this.organizationContentRoleRepository = new CAMCMSServer.Database.Repository.OrganizationContentRoleRepository(new MockDataContext());

        }

        [Test]
        public void NotNullTest()
        {
            var organizationContentRoleRepository = new CAMCMSServer.Database.Repository.OrganizationContentRoleRepository(new MockDataContext());
            Assert.NotNull(organizationContentRoleRepository);
        }
        #endregion
    }
}
