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
public class UpdateUserRolesTests
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
    public async Task UpdateValidUserRoleTest()
    {
        var newUserRole = new UserAccessRoleRequest
        {
            UserId = 1,
            ContextId = 1,
            AccessRoleId = 2
        };

        var updatedRole = new UserAccessRoleRequest
        {
            UserId = newUserRole.UserId,
            ContextId = newUserRole.ContextId,
            AccessRoleId = 4
        };

        await this.service.CreateUserRoles(newUserRole, 4);

        Assert.Contains(newUserRole, (ICollection?)MockDataContext.Users.Find(x => x.Id == newUserRole.UserId)?.Roles);

        var user = await this.service.UpdateUserRoles(updatedRole);

        var actual = user.Roles?.First(x =>
            x.ContextId == updatedRole.ContextId && x.AccessRoleId == updatedRole.AccessRoleId);

        Assert.NotNull(actual);
    }

    [Test]
    public async Task UpdateNonExistentContextRoleTest()
    {
        var newUserRole = new UserAccessRoleRequest
        {
            UserId = 1,
            ContextId = 1,
            AccessRoleId = 2
        };

        var updatedRole = new UserAccessRoleRequest
        {
            UserId = newUserRole.UserId,
            ContextId = 2,
            AccessRoleId = 4
        };

        await this.service.CreateUserRoles(newUserRole, 4);

        Assert.Contains(newUserRole, (ICollection?)MockDataContext.Users.Find(x => x.Id == newUserRole.UserId)?.Roles);

        var user = await this.service.UpdateUserRoles(updatedRole);

        Assert.Throws<InvalidOperationException>(() =>
            user.Roles?.First(x => x.ContextId == updatedRole.ContextId && x.AccessRoleId == updatedRole.AccessRoleId));
    }

    [Test]
    public async Task UpdateInvalidNullUserRoleTest()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.service.UpdateUserRoles(null));
    }

    #endregion
}