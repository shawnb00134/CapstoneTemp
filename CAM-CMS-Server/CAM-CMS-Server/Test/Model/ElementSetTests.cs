using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using CAMCMSServer.Model;
using CAMCMSServer.Model.Requests;
using NUnit.Framework;

#pragma warning disable CS8602

namespace CAMCMSServer.Test.Model;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ElementSetTests
{
    #region Methods

    [Test]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
        // Arrange
        var elementSet = new ElementSet { SetLocationId = 1 };
        var other = new object();

        // Act
        var result = elementSet.Equals(other);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_WithNullObject_ReturnsFalse()
    {
        // Arrange
        var elementSet = new ElementSet { SetLocationId = 1 };

        // Act
        var result = elementSet.Equals(null);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_WithSameSetLocationId_ReturnsTrue()
    {
        // Arrange
        var elementSet = new ElementSet { SetLocationId = 1 };
        var other = new ElementSet { SetLocationId = 1 };

        // Act
        var result = elementSet.Equals(other);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void Equals_WithDifferentSetLocationId_ReturnsFalse()
    {
        // Arrange
        var elementSet = new ElementSet { SetLocationId = 1 };
        var other = new ElementSet { SetLocationId = 2 };

        // Act
        var result = elementSet.Equals(other);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void Elements_DefaultValueIsNull()
    {
        // Arrange
        var elementSet = new ElementSet();

        // Act
        var elements = elementSet.Elements;

        // Assert
        Assert.IsNull(elements);
    }

    [Test]
    public void Test_SetStylingJsonValue()
    {
        var elementSet = new ElementSet();
        var styling = new SetStyling
        {
            is_appendix = false,
            is_horizontal = false,
            has_page_break = "false"
        };

        elementSet.StylingJson = JsonSerializer.Serialize(styling);

        Assert.AreEqual(styling.is_appendix, elementSet.Styling.is_appendix);
        Assert.AreEqual(styling.is_horizontal, elementSet.Styling.is_horizontal);
        Assert.AreEqual(styling.has_page_break, elementSet.Styling.has_page_break);
    }

    [Test]
    public void Test_SetNullStylingJsonValue()
    {
        var elementSet = new ElementSet
        {
            StylingJson = null
        };

        Assert.IsNotNull(elementSet.Styling);
        Assert.AreEqual(null, elementSet.Styling.is_appendix);
        Assert.AreEqual(null, elementSet.Styling.is_horizontal);
        Assert.AreEqual(null, elementSet.Styling.has_page_break);
    }

    [Test]
    public void Test_GetNullStylingJsonValue()
    {
        var elementSet = new ElementSet();

        Assert.IsNull(elementSet.Styling);
        Assert.IsNotNull(elementSet.StylingJson);

        var style = JsonSerializer.Deserialize<SetStyling>(elementSet.StylingJson ??
                                                           throw new InvalidOperationException());
        Assert.AreEqual(false, style.is_appendix);
        Assert.AreEqual("false", style.has_page_break);
        Assert.AreEqual(false, style.is_horizontal);
    }

    [Test]
    public void Test_SetPropertyValues()
    {
        // Arrange & Act
        var module = new ElementSet
        {
            Elements = Array.Empty<ElementLocation>(),
            SetLocationId = 1,
            ModuleId = 0,
            Place = 0,
            IsEditable = false,
            Styling = new SetStyling
            {
                is_appendix = false,
                is_horizontal = false,
                has_page_break = "false"
            }
        };

        // Assert
        Assert.IsEmpty(module.Elements);
        Assert.AreEqual(1, module.SetLocationId);
        Assert.AreEqual(0, module.ModuleId);
        Assert.AreEqual(0, module.Place);
        Assert.IsFalse(module.IsEditable);
        Assert.IsFalse(module.Styling.is_appendix);
        Assert.IsFalse(module.Styling.is_horizontal);
        Assert.IsFalse(module.Styling.has_page_break == "true");
        Assert.AreEqual(JsonSerializer.Serialize(module.Styling), module.StylingJson);
        Assert.NotNull(module.GetHashCode());
    }

    #endregion
}