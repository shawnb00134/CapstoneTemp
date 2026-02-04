using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Model;
using NUnit.Framework;

namespace CAMCMSServer.Test.Model;

[ExcludeFromCodeCoverage]
[TestFixture]
public class AccessRoleTests
{
    #region Methods

    [Test]
    public void TestInitialize()
    {
        var privileges = new List<Privilege>();

        var testAcessRole = new AccessRole
        {
            Id = 4,
            Name = "Role",
            Privileges = privileges
        };

        Assert.AreEqual(4, testAcessRole.Id);
        Assert.AreEqual("Role", testAcessRole.Name);
        Assert.AreEqual(privileges, testAcessRole.Privileges);
    }

    #endregion
}