using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8618

namespace CAMCMSServer.Test.Database.Repository.UserRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CreateUserTests
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
    public async Task AddOneValidUser()
    {
        var newUser = new User
        {
            Id = 3,
            Username = "Test",
            Password = "Test",
        };

        var oldSize = MockDataContext.Users.Count;

        await this.repository.CreateUser(newUser);

        Assert.AreEqual(MockDataContext.Users.Count, oldSize + 1);
        Assert.Contains(newUser, MockDataContext.Users);
    }

    [Test]
    public async Task AddTwoValidUsers()
    {
        var newUserOne = new User
        {
            Id = 3,
            Username = "Test",
            Password = "Test",
        };

        var newUserTwo = new User
        {
            Id = 4,
            Username = "Testers",
            Password = "Test",
        };

        var oldSize = MockDataContext.Users.Count;

        await this.repository.CreateUser(newUserOne);
        await this.repository.CreateUser(newUserTwo);

        Assert.AreEqual(MockDataContext.Users.Count, oldSize + 2);
        Assert.Contains(newUserOne, MockDataContext.Users);
        Assert.Contains(newUserTwo, MockDataContext.Users);
    }

    [Test]
    public void AddNullUser()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository.CreateUser(null!));
    }

    [Test]
    public void AddInvalidUsernameUser()
    {
        var newUser = new User
        {
            Id = 3,
            Password = "Test",
        };

        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository.CreateUser(newUser));
    }

    [Test]
    public void AddInvalidPasswordUser()
    {
        var newUser = new User
        {
            Id = 3,
            Username = "Test",
        };

        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository.CreateUser(newUser));
    }

    #endregion
}