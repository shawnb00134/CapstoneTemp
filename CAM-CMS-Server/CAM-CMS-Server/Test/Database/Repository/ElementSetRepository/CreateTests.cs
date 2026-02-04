using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Model.Requests;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8602

namespace CAMCMSServer.Test.Database.Repository.ElementSetRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CreateTests
{
    #region Data members

    private IElementSetRepository? repository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        this.repository = new CAMCMSServer.Database.Repository.ElementSetRepository(context);
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

        var result = await this.repository?.Create(newElementSet)!;

        Assert.NotNull(result);
        Assert.AreEqual(newElementSet, result);
        Assert.AreEqual(countBeforeAdd + 1, MockDataContext.Sets.Count);
        Assert.Contains(newElementSet, MockDataContext.Sets);
    }

    [Test]
    public void AddNullElement()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.Create(null!)!);
    }

    [Test]
    public void AddInvalidSetIdElement()
    {
        var newElementSet = new ElementSet
        {
            SetLocationId = -1,
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

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.Create(newElementSet));
    }

    [Test]
    public void AddInvalidModuleIdElement()
    {
        var newElementSet = new ElementSet
        {
            ModuleId = 0,
            Place = 3,
            IsEditable = false,
            Styling = new SetStyling
            {
                is_appendix = false,
                is_horizontal = false,
                has_page_break = "false"
            }
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.Create(newElementSet));
    }

    [Test]
    public void AddInvalidPlaceElement()
    {
        var newElementSet = new ElementSet
        {
            ModuleId = 1,
            Place = -1,
            IsEditable = false,
            Styling = new SetStyling
            {
                is_appendix = false,
                is_horizontal = false,
                has_page_break = "false"
            }
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.Create(newElementSet));
    }

    [Test]
    public void AddInvalidIsEditableElement()
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

        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.Create(newElementSet));
    }

    #endregion
}