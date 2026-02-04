using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.ContentRolesService
{

    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class ConstructorTests
    {
        [Test]
        public void TestNotNull()
        {
            var context = new MockDataContext();
            var repository = new ContentRoleRepository(context);
            var service = new ContentRoleService(repository);
            Assert.IsNotNull(service);
        }
    }
}
