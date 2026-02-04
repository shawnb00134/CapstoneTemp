/*using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8618

namespace CAMCMSServer.Test.Database.Repository.UserRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetDbUserTests
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
    public async Task GetValidUser()
    {
        var cognitoUser = new User
        {
            CognitoId = "Cognito-id-0"
        };

        var user = await this.repository.GetDbUser(cognitoUser);

        Assert.IsNotNull(user);
        Assert.AreEqual(MockDataContext.Users[0], user);
    }

    [Test]
    public async Task GetInvalidUser()
    {
        var cognitoUser = new User
        {
            CognitoId = "Cognito-id-6"
        };

        var user = await this.repository.GetDbUser(cognitoUser);

        Assert.IsNull(user);
    }

    #endregion
}*/