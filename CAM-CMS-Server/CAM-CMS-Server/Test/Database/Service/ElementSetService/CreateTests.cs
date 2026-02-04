using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Model.Requests;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;
//using Amazon.S3;

namespace CAMCMSServer.Test.Database.Service.ElementSetService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CreateTests
{
    #region Data members

    private IElementSetService? elementSetService;

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
    public async Task AddOneValidElement()
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

        var countBeforeAdd = MockDataContext.Sets.Count;

        await this.elementSetService?.Create(newElementSet)!;

        Assert.AreEqual(countBeforeAdd + 1, MockDataContext.Sets.Count);
        Assert.Contains(newElementSet, MockDataContext.Sets);
    }

    [Test]
    public async Task AddInvalidIsEditableElement()
    {
        var newElementSet = new ElementSet
        {
            ModuleId = 1,
            Place = 3,
            Styling = new SetStyling
            {
                is_appendix = false,
                is_horizontal = false,
                has_page_break = "false"
            }
        };

        var countBeforeAdd = MockDataContext.Sets.Count;

        await this.elementSetService?.Create(newElementSet)!;

        Assert.AreEqual(countBeforeAdd + 1, MockDataContext.Sets.Count);
        Assert.Contains(newElementSet, MockDataContext.Sets);
    }

    [Test]
    public async Task AddInvalidIsAppendixElement()
    {
        var newElementSet = new ElementSet
        {
            ModuleId = 1,
            Place = 3,
            IsEditable = false,
            Styling = new SetStyling
            {
                is_horizontal = false,
                has_page_break = "false"
            }
        };

        var countBeforeAdd = MockDataContext.Sets.Count;

        await this.elementSetService?.Create(newElementSet)!;

        Assert.AreEqual(countBeforeAdd + 1, MockDataContext.Sets.Count);
        Assert.Contains(newElementSet, MockDataContext.Sets);
    }

    [Test]
    public void GetInvalidElement()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.elementSetService?.Create(null!)!);
    }

    [Test]
    public async Task AddInvalidStyleElement()
    {
        var newElementSet = new ElementSet
        {
            ModuleId = 1,
            Place = 3,
            IsEditable = false
        };

        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.elementSetService?.Create(newElementSet)!);
    }

    #endregion
}