using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Controller.ContentRoleController
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class GetTests
    {
        #region Data members

        private IContentRoleRepository repository;  

        private IContentRoleService service;

        private IUserService userService;

        private CAMCMSServer.Controller.ContentRoleController controller;

        #endregion


        #region Methods

        [SetUp]
        public void Setup()
        {
            this.repository = new ContentRoleRepository(new MockDataContext());
            this.userService = new UserService(new UserRepository(new MockDataContext()), new InvitationRepository(new MockDataContext()));
            this.service = new ContentRoleService(this.repository);
            this.controller = new CAMCMSServer.Controller.ContentRoleController(this.service, this.userService);
        }

        [Test]
        public void GetAllContentRolesValid()
        {
            var userId = 1;
            var result = this.controller.GetAll(userId, "").Result;
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void GetAllContentRolesInvalidUserId()
        {
            var userId = 0;
            var result = this.controller.GetAll(userId, "").Result;
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
        #endregion
    }
}
