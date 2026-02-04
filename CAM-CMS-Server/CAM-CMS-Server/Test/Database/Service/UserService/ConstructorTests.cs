using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8618

namespace CAMCMSServer.Test.Database.Service.UserService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ConstructorTests
{
    #region Data members

    private IUserRepository repository;

    private IInvitationRepository invitationRepository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        this.repository = new UserRepository(context);
    }

    [Test]
    public void NotNullTest()
    {
        var authRepo = new CAMCMSServer.Database.Service.UserService(this.repository, this.invitationRepository);

        Assert.IsNotNull(authRepo);
    }

    #endregion
}