using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.UserRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetUserByIdTests
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
    public async Task GetValidUser()
    {
        var id = 1;

        var user = await this.repository.GetUserById(id);

        var expected = MockDataContext.Users.Where(x => x.Id == id).ElementAt(0);

        Assert.IsNotNull(user);
        Assert.AreEqual(expected, user);
    }

    [Test]
    public async Task GetInvalidUser()
    {
        var id = 500;

        var user = await this.repository.GetUserById(id);

        Assert.IsNotNull(user);
        Assert.AreEqual(null, user.Id);
    }

    [Test]
    public async Task GetInvalidId()
    {
        var id = 0;

        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await this.repository.GetUserById(id));
    }

    #endregion
}