using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Model;
using NUnit.Framework;

namespace CAMCMSServer.Test.Model;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ModuleTests
{
    #region Methods

    [Test]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
        // Arrange
        var module = new Module { ModuleId = 1 };
        var other = new object();

        // Act
        var result = module.Equals(other);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_WithNullObject_ReturnsFalse()
    {
        // Arrange
        var module = new Module { ModuleId = 1 };

        // Act
        var result = module.Equals(null);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_WithSameModuleId_ReturnsTrue()
    {
        // Arrange
        var module = new Module { ModuleId = 1 };
        var other = new Module { ModuleId = 1 };

        // Act
        var result = module.Equals(other);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void Equals_WithDifferentModuleId_ReturnsFalse()
    {
        // Arrange
        var module = new Module { ModuleId = 1 };
        var other = new Module { ModuleId = 2 };

        // Act
        var result = module.Equals(other);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void ElementSets_DefaultValueIsNull()
    {
        // Arrange
        var module = new Module();

        // Act
        var elementSets = module.ElementSets;

        // Assert
        Assert.IsNull(elementSets);
    }

    [Test]
    public void Tags_DefaultValueIsNull()
    {
        // Arrange
        var module = new Module();

        // Act
        var tags = module.Tags;

        // Assert
        Assert.IsNull(tags);
    }

    [Test]
    public void Test_SetPropertyValues()
    {
        // Arrange & Act
        var module = new Module
        {
            ElementSets = Array.Empty<ElementSet>(),
            ModuleId = 1,
            Title = "Testing Module",
            Description = "Testing Module",
            SurveyStartLink = "Testing Module",
            SurveyEndLink = "Testing Module",
            ContactTitle = "Testing Module",
            ContactNumber = "Testing Module",
            Thumbnail = "Testing Module",
            Tags = Array.Empty<string>(),
            DisplayTitle = "Testing Module",
            TemplateModuleId = 0,
            IsTemplate = false,
            PublishedTime = "PUBLISHED",
            CreatedAt = "CREATED",
            LibraryFolderId = 0,
            CreatedBy = 0
        };

        // Assert
        Assert.IsEmpty(module.ElementSets);
        Assert.AreEqual(1, module.ModuleId);
        Assert.AreEqual("Testing Module", module.Title);
        Assert.AreEqual("Testing Module", module.Description);
        Assert.AreEqual("Testing Module", module.SurveyStartLink);
        Assert.AreEqual("Testing Module", module.SurveyEndLink);
        Assert.AreEqual("Testing Module", module.ContactTitle);
        Assert.AreEqual("Testing Module", module.ContactNumber);
        Assert.AreEqual("Testing Module", module.Thumbnail);
        Assert.IsEmpty(module.Tags);
        Assert.AreEqual("Testing Module", module.DisplayTitle);
        Assert.AreEqual(0, module.TemplateModuleId);
        Assert.IsFalse(module.IsTemplate);
        Assert.AreEqual("PUBLISHED", module.PublishedTime);
        Assert.AreEqual("CREATED", module.CreatedAt);
        Assert.AreEqual(0, module.LibraryFolderId);
        Assert.AreEqual(0, module.CreatedBy);
        Assert.NotNull(module.GetHashCode());
    }

    #endregion
}