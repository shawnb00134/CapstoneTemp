/*using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8618

namespace CAMCMSServer.Test.Database.Service.UserService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ValidateUserTests
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
    public async Task ValidateValidUser()
    {
        var cognitoUser = new User
        {
            CognitoId = "Cognito-id-0"
        };

        var user = await this.service.ValidateUser(cognitoUser);

        Assert.IsNotNull(user);
        Assert.AreEqual(MockDataContext.Users[0], user);
    }

    [Test]
    public async Task AddUnknownUser()
    {
        var cognitoUser = new User
        {
            Id = 3,
            Username = "Test",
            Password = "Test",
            CognitoId = "Cognito-id-7"
        };

        var user = await this.service.ValidateUser(cognitoUser);

        Assert.IsNotNull(user);
        Assert.Contains(cognitoUser, MockDataContext.Users);
    }

    [Test]
    public void ValidateNullUser()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.service.ValidateUser(null!));
    }

    [Test]
    public void ValidateNullCognitoIdUser()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.service.ValidateUser(new User()));
    }

    #endregion
}*/