using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Controller.ContentRoleController
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
            var userService = new UserService(new UserRepository(context), new InvitationRepository(context));
            var controller = new CAMCMSServer.Controller.ContentRoleController(service, userService);
            Assert.IsNotNull(controller);
        }
    }
}
