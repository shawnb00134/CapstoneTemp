using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Model;
using NUnit.Framework;

namespace CAMCMSServer.Test.Model;

[ExcludeFromCodeCoverage]
[TestFixture]
public class PrivilegeTests
{
    #region Methods

    [Test]
    public void TestInitialize()
    {
        var testAcessRole = new Privilege
        {
            Id = 4,
            Name = "Privilege"
        };

        Assert.AreEqual(4, testAcessRole.Id);
        Assert.AreEqual("Privilege", testAcessRole.Name);
    }

    #endregion
}