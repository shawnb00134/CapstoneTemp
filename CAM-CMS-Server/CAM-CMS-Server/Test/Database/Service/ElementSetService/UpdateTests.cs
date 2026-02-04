using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Model.Requests;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;
//using Amazon.S3;

#pragma warning disable CS8618
#pragma warning disable CS8602

namespace CAMCMSServer.Test.Database.Service.ElementSetService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class UpdateTests
{
    #region Data members

    private IElementSetService elementSetService;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        var elementRepository = new ElementRepository(context);
        var elementService = new CAMCMSServer.Database.Service.ElementService(elementRepository);

        var elementSetRepository = new ElementSetRepository(context);
        //this.elementSetService = new CAMCMSServer.Database.Service.ElementSetService(elementSetRepository, elementService,
        //    new FileService(new AmazonS3Client(), ""));
    }

    [Test]
    public async Task UpdateOneValidElement()
    {
        var newElementSet = new ElementSet
        {
            SetLocationId = 2,
            ModuleId = 1,
            Place = 3,
            IsEditable = false,
            Styling = new SetStyling
            {
                is_appendix = false,
                is_horizontal = false,
                has_page_break = "false"
            }
        };

        await this.elementSetService.Update(newElementSet);

        var actual = MockDataContext.Sets.Where(x => x.SetLocationId == newElementSet.SetLocationId).ElementAt(0);

        Assert.AreEqual(newElementSet.SetLocationId, actual.SetLocationId);
        Assert.AreEqual(newElementSet.ModuleId, actual.ModuleId);
        Assert.AreEqual(newElementSet.Place, actual.Place);
        Assert.AreEqual(newElementSet.IsEditable, actual.IsEditable);
        Assert.AreEqual(newElementSet.Styling.is_appendix, actual.Styling.is_appendix);
        Assert.AreEqual(newElementSet.Styling.is_horizontal, actual.Styling.is_horizontal);
        Assert.AreEqual(newElementSet.Styling.has_page_break, actual.Styling.has_page_break);
    }

    [Test]
    public async Task UpdateInvalidSetLocationIdElement()
    {
        var newElementSet = new ElementSet
        {
            ModuleId = 1,
            Place = 3,
            IsEditable = false,
            Styling = new SetStyling
            {
                is_appendix = false,
                is_horizontal = false,
                has_page_break = "false"
            }
        };

        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.elementSetService.Update(newElementSet)!);
    }

    [Test]
    public async Task UpdateValidSetLocationAttributes()
    {
        var newElementLocation = new ElementLocation
        {
            SetLocationId = 1,
            ElementId = 2,
            Place = 0,
            IsEditable = false,
            Attributes = new SetLocationAttributes
            {
                Width = "200",
                Height = "auto",
                Alignment = "left"
            }
        };
        var oldWidth = "auto";
        await this.elementSetService.UpdateElementLocationAttribute(newElementLocation);

        var actual = MockDataContext.Locations.Where(x => x.SetLocationId == newElementLocation.SetLocationId)
            .ElementAt(0);
        Assert.AreEqual(1, actual.SetLocationId);
        Assert.AreEqual(2, actual.ElementId);
        Assert.AreEqual(0, actual.Place);
        Assert.AreNotEqual(oldWidth, actual.Attributes.Width);
        Assert.AreEqual("auto", actual.Attributes.Height);
        Assert.AreEqual("left", actual.Attributes.Alignment);
    }

    #endregion
}