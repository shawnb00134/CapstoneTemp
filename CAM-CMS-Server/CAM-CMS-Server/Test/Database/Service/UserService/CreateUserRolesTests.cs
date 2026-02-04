using System.Collections;
using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model.Requests;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.UserService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CreateUserRolesTests
{
    #region Data members

    private IUserService service;

    private IInvitationRepository invitationRepository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        var repository = new UserRepository(context);

        this.service = new CAMCMSServer.Database.Service.UserService(repository, this.invitationRepository);
    }

    [Test]
    public async Task CreateValidUserRoleTest()
    {
        var newUserRole = new UserAccessRoleRequest
        {
            UserId = 1,
            ContextId = 1,
            AccessRoleId = 2
        };

        var userRolesBefore = MockDataContext.Users.Find(x => x.Id == newUserRole.UserId)?.Roles;
        var countBefore = userRolesBefore?.Count() ?? 0;

        var user = await this.service.CreateUserRoles(newUserRole, 4);

        var countAfter = user.Roles?.Count() ?? 0;

        Assert.AreEqual(countBefore + 1, countAfter);
        Assert.Contains(newUserRole, (ICollection?)user.Roles);
    }

    [Test]
    public async Task CreateInvalidUserIdRoleTest()
    {
        var newUserRole = new UserAccessRoleRequest
        {
            UserId = -1,
            ContextId = 1,
            AccessRoleId = 2
        };

        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await this.service.CreateUserRoles(newUserRole, 4));
    }

    [Test]
    public async Task CreateInvalidNullUserRoleTest()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.service.CreateUserRoles(null, 4));
    }

    #endregion
}