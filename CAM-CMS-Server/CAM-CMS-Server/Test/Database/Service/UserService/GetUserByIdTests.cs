using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.UserService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetUserByIdTests
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
    public async Task GetValidUser()
    {
        var user = await this.service.GetUserById(1);

        Assert.IsNotNull(user);
        Assert.AreEqual(MockDataContext.Users.Find(x => x.Id == 1), user);
    }

    [Test]
    public async Task GetInvalidUser()
    {
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await this.service.GetUserById(-1));
    }

    #endregion
}