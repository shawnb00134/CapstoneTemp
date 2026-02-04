using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.ContentRoleRepository
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class ConstructorTests
    {
        [Test]
        public void TestNotNull()
        {
            var context = new MockDataContext();
            var repository = new CAMCMSServer.Database.Repository.ContentRoleRepository(context);
            Assert.IsNotNull(repository);
        }
    }
}
