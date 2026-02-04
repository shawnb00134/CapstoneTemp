using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Model;
using CAMCMSServer.Model.ElementTypes;
using NUnit.Framework;

namespace CAMCMSServer.Test.Model;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ElementTests
{
    #region Methods

    [Test]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
        // Arrange
        var element = new Element { ElementId = 1 };
        var other = new object();

        // Act
        var result = element.Equals(other);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_WithNullObject_ReturnsFalse()
    {
        // Arrange
        var element = new Element { ElementId = 1 };

        // Act
        var result = element.Equals(null);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_WithSameElementId_ReturnsTrue()
    {
        // Arrange
        var element = new Element { ElementId = 1 };
        var other = new Element { ElementId = 1 };

        // Act
        var result = element.Equals(other);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void Equals_WithDifferentElementId_ReturnsFalse()
    {
        // Arrange
        var element = new Element { ElementId = 1 };
        var other = new Element { ElementId = 2 };

        // Act
        var result = element.Equals(other);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void Test_SetPropertyValues()
    {
        var imageElement = new ImageElement();

        // Arrange & Act
        var element = new Element
        {
            ElementId = 1,
            Title = "Testing Element",
            Description = "Testing Element",
            TypeId = 0,
            Citation = "Testing Element",
            Content = "{}",
            ExternalSource = "SOURCE",
            Tags = Array.Empty<string>(),
            CreatedAt = "CREATED",
            LibraryFolderId = 0,
            ConfirmedDelete = false,
            CreatedBy = 4,
            FormFile = null,
            ElementType = imageElement
        };

        // Assert
        Assert.AreEqual(1, element.ElementId);
        Assert.AreEqual("Testing Element", element.Title);
        Assert.AreEqual("Testing Element", element.Description);
        Assert.AreEqual(0, element.TypeId);
        Assert.AreEqual("Testing Element", element.Citation);
        Assert.AreEqual("{}", element.Content);
        Assert.AreEqual("SOURCE", element.ExternalSource);
        Assert.IsEmpty(element.Tags);
        Assert.AreEqual("CREATED", element.CreatedAt);
        Assert.AreEqual(0, element.LibraryFolderId);
        Assert.IsFalse(element.ConfirmedDelete);
        Assert.AreEqual(4, element.CreatedBy);
        Assert.AreEqual(imageElement, element.ElementType);
        Assert.IsNull(element.FormFile);
        Assert.NotNull(element.GetHashCode());
    }

    #endregion
}