using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Model;
using NUnit.Framework;

namespace CAMCMSServer.Test.Model;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ContextTests
{
    #region Methods

    [Test]
    public void TestInitialize()
    {
        var testContext = new Context
        {
            Id = 4,
            Type = "library folder",
            Instance = 4,
            InstanceName = "Test Folder"
        };

        Assert.AreEqual(4, testContext.Id);
        Assert.AreEqual("library folder", testContext.Type);
        Assert.AreEqual(4, testContext.Instance);
        Assert.AreEqual("Test Folder", testContext.InstanceName);
    }

    #endregion
}