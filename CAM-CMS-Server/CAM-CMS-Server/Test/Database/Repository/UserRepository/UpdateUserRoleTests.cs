using System.Collections;
using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model.Requests;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.UserRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class UpdateUserRoleTests
{
    #region Data members

    private IUserRepository repository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        this.repository = new CAMCMSServer.Database.Repository.UserRepository(context);
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

        await this.repository.CreateUserRole(newUserRole);

        Assert.Contains(newUserRole, (ICollection?)MockDataContext.Users.Find(x => x.Id == newUserRole.UserId)?.Roles);

        await this.repository.UpdateUserRole(updatedRole);

        var roles = MockDataContext.Users.Find(x => x.Id == newUserRole.UserId)?.Roles ??
                    throw new ArgumentNullException();
        var actual =
            roles.First(x => x.ContextId == updatedRole.ContextId && x.AccessRoleId == updatedRole.AccessRoleId);

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

        await this.repository.CreateUserRole(newUserRole);

        Assert.Contains(newUserRole, (ICollection?)MockDataContext.Users.Find(x => x.Id == newUserRole.UserId)?.Roles);

        await this.repository.UpdateUserRole(updatedRole);

        var roles = MockDataContext.Users.Find(x => x.Id == newUserRole.UserId)?.Roles ??
                    throw new ArgumentNullException();

        Assert.Throws<InvalidOperationException>(() =>
            roles.First(x => x.ContextId == updatedRole.ContextId && x.AccessRoleId == updatedRole.AccessRoleId));
    }

    [Test]
    public async Task UpdateInvalidNullUserRoleTest()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository.UpdateUserRole(null));
    }

    #endregion
}