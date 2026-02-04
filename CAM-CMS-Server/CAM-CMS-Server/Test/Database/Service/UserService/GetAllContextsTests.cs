using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.UserService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetAllContextsTests
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
    public async Task GetAllContexts()
    {
        var contexts = await this.service.GetAllContexts();

        var expected = MockDataContext.Contexts;

        Assert.AreEqual(expected, contexts);
    }

    #endregion
}