using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.ContentRoleRepository
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class GetTests
    {
        #region MyRegion
        private IContentRoleRepository repository;
        #endregion
        #region Methods

        [SetUp]
        public void Setup()
        {
            this.repository = new CAMCMSServer.Database.Repository.ContentRoleRepository(new MockDataContext());
        }

        [Test]
        public void GetAllContentRoles()
        {
            var result = this.repository.GetAllContentRoles().Result;
            var amount = MockDataContext.ContentRoles.Count;
            Assert.IsNotNull(result);
            Assert.AreEqual(amount, result.Count());

        }
        #endregion
    }
}
