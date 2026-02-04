using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.UserRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetRolePrivilegesTests
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
    public async Task GetRolePrivileges()
    {
        const int roleId = 2;

        var contexts = await this.repository.GetRolePrivileges(new AccessRole { Id = roleId });

        var expected = MockDataContext.Roles.Find(x => x.Id == roleId).Privileges;

        Assert.AreEqual(expected, contexts);
    }

    [Test]
    public async Task GetNonExistentRolePrivileges()
    {
        const int roleId = -5;

        var contexts = await this.repository.GetRolePrivileges(new AccessRole { Id = roleId });

        var expectedRole = MockDataContext.Roles.Find(x => x.Id == roleId);
        var expected = expectedRole?.Privileges ?? new List<Privilege>();

        Assert.AreEqual(expected, contexts);
    }

    [Test]
    public async Task GetInvalidRolePrivileges()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository.GetRolePrivileges(null));
    }

    #endregion
}