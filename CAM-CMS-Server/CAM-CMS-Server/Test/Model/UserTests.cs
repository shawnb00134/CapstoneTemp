/*using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Model;
using CAMCMSServer.Model.Requests;
using NUnit.Framework;

namespace CAMCMSServer.Test.Model;

[ExcludeFromCodeCoverage]
[TestFixture]
public class UserTests
{
    #region Methods

    [Test]
    public void Test_SetPropertyValues()
    {
        var roles = new List<UserAccessRoleRequest>();

        // Arrange & Act
        var user = new User
        {
            Id = 1,
            CognitoId = "Cognito-id-1",
            AccessToken = "AccessToken",
            RefreshToken = "RefreshToken",
            Timestamp = "Start time",
            Username = "Test Username",
            Password = "Test Password",
            Firstname = "Test First Name",
            Lastname = "Test Last Name",
            Email = "Test Email",
            Phone = "Test Phone",
            Roles = roles
        };

        // Assert
        Assert.AreEqual(1, user.Id);
        Assert.AreEqual("Cognito-id-1", user.CognitoId);
        Assert.AreEqual("AccessToken", user.AccessToken);
        Assert.AreEqual("RefreshToken", user.RefreshToken);
        Assert.AreEqual("Start time", user.Timestamp);
        Assert.AreEqual("Test Username", user.Username);
        Assert.AreEqual("Test Password", user.Password);
        Assert.AreEqual("Test First Name", user.Firstname);
        Assert.AreEqual("Test Last Name", user.Lastname);
        Assert.AreEqual("Test Email", user.Email);
        Assert.AreEqual("Test Phone", user.Phone);
        Assert.AreEqual(roles, user.Roles);
        Assert.NotNull(user.GetHashCode());
    }

    [Test]
    public void Test_TrueEqualsMethod()
    {
        var userOne = new User
        {
            Id = 1
        };

        var userTwo = new User
        {
            Id = 1
        };

        Assert.IsTrue(userOne.Equals(userTwo));
    }

    [Test]
    public void Test_FalseEqualsMethod()
    {
        var userOne = new User
        {
            Id = 1
        };

        var userTwo = new User
        {
            Id = 2
        };

        Assert.IsFalse(userOne.Equals(userTwo));
    }

    [Test]
    public void Test_NullEqualsMethod()
    {
        var userOne = new User
        {
            Id = 1
        };

        Assert.IsFalse(userOne.Equals(null));
    }

    [Test]
    public void Test_InvalidObjectEqualsMethod()
    {
        var userOne = new User
        {
            Id = 1
        };

        // ReSharper disable once SuspiciousTypeConversion.Global
        Assert.IsFalse(userOne.Equals(200));
    }

    #endregion
}*/