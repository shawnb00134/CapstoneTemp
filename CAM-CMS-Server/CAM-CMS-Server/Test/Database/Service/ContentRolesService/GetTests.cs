using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.ContentRolesService
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class GetTests
    {
        #region MyRegion
        private IContentRoleRepository repository;
        private IContentRoleService service;
        #endregion
        #region Methods

        [SetUp]
        public void Setup()
        {
            this.repository = new ContentRoleRepository(new MockDataContext());
            this.service = new ContentRoleService(this.repository);
        }

        [Test]
        public void GetAllContentRoles()
        {
            var result = this.service.GetAllContentRoles().Result;
            var amount = MockDataContext.ContentRoles.Count;
            Assert.IsNotNull(result);
            Assert.AreEqual(amount, result.Count());

        }
        #endregion
    }
}
