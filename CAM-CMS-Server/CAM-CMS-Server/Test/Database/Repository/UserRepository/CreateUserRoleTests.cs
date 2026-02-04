using System.Collections;
using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model.Requests;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.UserRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CreateUserRoleTests
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

        await this.repository.CreateUserRole(newUserRole);

        var userRolesAfter = MockDataContext.Users.Find(x => x.Id == newUserRole.UserId)?.Roles;
        var countAfter = userRolesAfter?.Count() ?? 0;

        Assert.AreEqual(countBefore + 1, countAfter);
        Assert.Contains(newUserRole, (ICollection?)userRolesAfter);
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

        var userRolesBefore = MockDataContext.Users.Find(x => x.Id == newUserRole.UserId)?.Roles;
        var countBefore = userRolesBefore?.Count() ?? 0;

        await this.repository.CreateUserRole(newUserRole);

        var userRolesAfter = MockDataContext.Users.Find(x => x.Id == newUserRole.UserId)?.Roles;
        var countAfter = userRolesAfter?.Count() ?? 0;

        Assert.AreEqual(countBefore, countAfter);
    }

    [Test]
    public async Task CreateInvalidNullUserRoleTest()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository.CreateUserRole(null));
    }

    #endregion
}