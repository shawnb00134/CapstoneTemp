using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Model;
using CAMCMSServer.Model.Requests;
using NUnit.Framework;

namespace CAMCMSServer.Test.Model;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ElementLocationTests
{
    #region Methods

    [Test]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
        // Arrange
        var elementLocation = new ElementLocation { SetLocationId = 1, ElementId = 1 };
        var other = new object();

        // Act
        var result = elementLocation.Equals(other);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_WithNullObject_ReturnsFalse()
    {
        // Arrange
        var elementLocation = new ElementLocation { SetLocationId = 1, ElementId = 1 };

        // Act
        var result = elementLocation.Equals(null);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_WithSameSetLocationIdAndElementId_ReturnsTrue()
    {
        // Arrange
        var elementLocation = new ElementLocation { SetLocationId = 1, ElementId = 1 };
        var other = new ElementLocation { SetLocationId = 1, ElementId = 1 };

        // Act
        var result = elementLocation.Equals(other);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void Equals_WithDifferentSetLocationId_ReturnsFalse()
    {
        // Arrange
        var elementLocation = new ElementLocation { SetLocationId = 1, ElementId = 1 };
        var other = new ElementLocation { SetLocationId = 2, ElementId = 1 };

        // Act
        var result = elementLocation.Equals(other);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_WithDifferentElementId_ReturnsFalse()
    {
        // Arrange
        var elementLocation = new ElementLocation { SetLocationId = 1, ElementId = 1 };
        var other = new ElementLocation { SetLocationId = 1, ElementId = 2 };

        // Act
        var result = elementLocation.Equals(other);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void Element_DefaultValueIsNull()
    {
        // Arrange
        var elementLocation = new ElementLocation();

        // Act
        var element = elementLocation.Element;

        // Assert
        Assert.IsNull(element);
    }

    [Test]
    public void Test_SetPropertyValues()
    {
        // Arrange & Act
        var module = new ElementLocation
        {
            Element = new Element(),
            SetLocationId = 1,
            ElementId = 0,
            Place = 0,
            IsEditable = false
        };

        // Assert
        Assert.NotNull(module.Element);
        Assert.AreEqual(1, module.SetLocationId);
        Assert.AreEqual(0, module.ElementId);
        Assert.AreEqual(0, module.Place);
        Assert.IsFalse(module.IsEditable);
        Assert.NotNull(module.GetHashCode());
    }

    [Test]
    public void Test_SetAttributeValues()
    {
        // Arrange & Act
        var module = new ElementLocation
        {
            Element = new Element(),
            SetLocationId = 1,
            ElementId = 0,
            Place = 0,
            IsEditable = false,
            Attributes = new SetLocationAttributes
            {
                Width = "auto",
                Height = "auto",
                Alignment = "left"
            }
        };

        // Assert
        Assert.NotNull(module.Element);
        Assert.AreEqual(1, module.SetLocationId);
        Assert.AreEqual(0, module.ElementId);
        Assert.AreEqual(0, module.Place);
        Assert.AreEqual("auto", module.Attributes.Width);
        Assert.AreEqual("auto", module.Attributes.Height);
        Assert.AreEqual("left", module.Attributes.Alignment);
        Assert.IsFalse(module.IsEditable);
        Assert.NotNull(module.GetHashCode());
    }

    #endregion
}