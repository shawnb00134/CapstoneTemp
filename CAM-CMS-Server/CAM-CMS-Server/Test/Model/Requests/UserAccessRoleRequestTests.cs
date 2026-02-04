using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Model.Requests;
using NUnit.Framework;

namespace CAMCMSServer.Test.Model.Requests;

[ExcludeFromCodeCoverage]
[TestFixture]
public class UserAccessRoleRequestTests
{
    #region Methods

    [Test]
    public void TestInitialize()
    {
        var testAcessRole = new UserAccessRoleRequest
        {
            UserId = 4,
            ContextId = 4,
            AccessRoleId = 4,
            CreatedBy = 4
        };

        Assert.AreEqual(4, testAcessRole.UserId);
        Assert.AreEqual(4, testAcessRole.ContextId);
        Assert.AreEqual(4, testAcessRole.AccessRoleId);
        Assert.AreEqual(4, testAcessRole.CreatedBy);
    }

    #endregion
}