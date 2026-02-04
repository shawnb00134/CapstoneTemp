using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Model;
using NUnit.Framework;

namespace CAMCMSServer.Test.Model;

[ExcludeFromCodeCoverage]
[TestFixture]
public class PublishedModuleTests
{
    #region Methods

    [Test]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
        var module = new PublishedModule { Id = 1 };
        var other = new object();

        var result = module.Equals(other);

        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_WithNullObject_ReturnsFalse()
    {
        var module = new PublishedModule { Id = 1 };

        var result = module.Equals(null);

        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_WithSameModuleId_ReturnsTrue()
    {
        var module = new PublishedModule { Id = 1 };
        var other = new PublishedModule { Id = 1 };

        var result = module.Equals(other);

        Assert.IsTrue(result);
    }

    [Test]
    public void Equals_WithDifferentModuleId_ReturnsFalse()
    {
        var module = new PublishedModule { Id = 1 };
        var other = new PublishedModule { Id = 2 };

        var result = module.Equals(other);

        Assert.IsFalse(result);
    }

    [Test]
    public void Cache_DefaultValueIsNull()
    {
        var module = new PublishedModule { Id = 1 };

        var cache = module.Cache;

        Assert.IsNull(cache);
    }

    [Test]
    public void TestSetProperties()
    {
        var module = new PublishedModule
        {
            Id = 1,
            Cache = "Test"
        };

        Assert.AreEqual("Test", module.Cache);
        Assert.AreEqual(1, module.Id);
    }

    #endregion
}