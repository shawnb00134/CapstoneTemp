using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.UserRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetUsersTests
{
    #region Data members

    private CAMCMSServer.Database.Repository.UserRepository repository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();
        this.repository = new CAMCMSServer.Database.Repository.UserRepository(context);
    }

    [Test]
    public async Task GetAllUsers()
    {
        var users = await this.repository.GetUsers();

        Assert.IsNotNull(users);
        Assert.AreEqual(MockDataContext.Users, users);
    }

    #endregion
}