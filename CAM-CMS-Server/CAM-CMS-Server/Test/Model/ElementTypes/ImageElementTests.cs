using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using CAMCMSServer.Model.ElementTypes;
using NUnit.Framework;

namespace CAMCMSServer.Test.Model.ElementTypes;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ImageElementTests
{
    #region Methods

    [Test]
    public void TestInitialize()
    {
        var testElement = new ImageElement
        {
            Key = "Key",
            Source = "Link"
        };

        Assert.AreEqual(ElementType.Image, testElement.ElementType);
        Assert.AreEqual("Key", testElement.Key);
        Assert.AreEqual("Link", testElement.Source);
    }

    [Test]
    public void TestConvertToJsonForPersistence()
    {
        var testElement = new ImageElement
        {
            Key = "Key",
            Source = "Link"
        };

        var json = new ImageElementPersistenceJson
        {
            Key = "Key"
        };

        Assert.AreEqual(JsonSerializer.SerializeToDocument(json).ToString(),
            testElement.ConvertToJsonForPersistence().ToString());
    }

    [Test]
    public void TestConvertFromJsonForPersistence()
    {
        var testElement = new ImageElement
        {
            Key = "Key",
            Source = "Link"
        };

        var json = JsonSerializer.SerializeToDocument(testElement);
        var testActualElement = new ImageElement();
        testActualElement.ConvertFromJsonForPersistence(json);

        Assert.AreEqual(testElement.ElementType, testActualElement.ElementType);
        Assert.AreEqual(testElement.Source, testActualElement.Source);
        Assert.AreEqual(testElement.Key, testActualElement.Key);
    }

    [Test]
    public void TestConvertToJsonForUI()
    {
        var testElement = new ImageElement
        {
            Key = "Key",
            Source = "Link"
        };

        Assert.AreEqual(JsonSerializer.SerializeToDocument(testElement).ToString(),
            testElement.ConvertToJsonForUI().ToString());
    }

    #endregion
}