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
public class UpdateTests
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
    public async Task UpdateOneValidElement()
    {
        var newElementSet = new ElementSet
        {
            SetLocationId = 1,
            ModuleId = 1,
            Place = 0,
            IsEditable = false,
            Styling = new SetStyling
            {
                is_appendix = false,
                is_horizontal = false,
                has_page_break = "false"
            },
            Elements = new List<ElementLocation>
            {
                new()
                {
                    SetLocationId = 1,
                    ElementId = 2,
                    Place = 0,
                    IsEditable = false
                },
                new()
                {
                    SetLocationId = 1,
                    ElementId = 1,
                    Place = 1,
                    IsEditable = false
                }
            }
        };

        var result = await this.repository?.Update(newElementSet)!;

        Assert.NotNull(result);
        Assert.AreEqual(newElementSet, result);

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
    public async Task UpdateTwoValidElements()
    {
        var newElementSetOne = new ElementSet
        {
            SetLocationId = 1,
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
        var newElementSetTwo = new ElementSet
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

        var result = await this.repository?.Update(newElementSetOne)!;

        Assert.NotNull(result);
        Assert.AreEqual(newElementSetOne, result);

        var actual = MockDataContext.Sets.Where(x => x.SetLocationId == newElementSetOne.SetLocationId).ElementAt(0);

        Assert.AreEqual(newElementSetOne.SetLocationId, actual.SetLocationId);
        Assert.AreEqual(newElementSetOne.ModuleId, actual.ModuleId);
        Assert.AreEqual(newElementSetOne.Place, actual.Place);
        Assert.AreEqual(newElementSetOne.IsEditable, actual.IsEditable);
        Assert.AreEqual(newElementSetOne.Styling.is_appendix, actual.Styling.is_appendix);
        Assert.AreEqual(newElementSetOne.Styling.is_horizontal, actual.Styling.is_horizontal);
        Assert.AreEqual(newElementSetOne.Styling.has_page_break, actual.Styling.has_page_break);

        result = await this.repository?.Update(newElementSetTwo)!;

        Assert.NotNull(result);
        Assert.AreEqual(newElementSetTwo, result);

        actual = MockDataContext.Sets.Where(x => x.SetLocationId == newElementSetTwo.SetLocationId).ElementAt(0);

        Assert.AreEqual(newElementSetTwo.SetLocationId, actual.SetLocationId);
        Assert.AreEqual(newElementSetTwo.ModuleId, actual.ModuleId);
        Assert.AreEqual(newElementSetTwo.Place, actual.Place);
        Assert.AreEqual(newElementSetTwo.IsEditable, actual.IsEditable);
        Assert.AreEqual(newElementSetTwo.Styling.is_appendix, actual.Styling.is_appendix);
        Assert.AreEqual(newElementSetTwo.Styling.is_horizontal, actual.Styling.is_horizontal);
        Assert.AreEqual(newElementSetTwo.Styling.has_page_break, actual.Styling.has_page_break);
    }

    [Test]
    public void UpdateNullElement()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.Update(null!)!);
    }

    [Test]
    public void UpdateInvalidSetIdElement()
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

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.Update(newElementSet));
    }

    [Test]
    public void UpdateInvalidModuleIdElement()
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

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.Update(newElementSet));
    }

    [Test]
    public void UpdateInvalidPlaceElement()
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

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.Update(newElementSet));
    }

    [Test]
    public void UpdateInvalidIsEditableElement()
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

        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.Update(newElementSet));
    }

    [Test]
    public async Task UpdateInvalidLocationElement()
    {
        var newElementSet = new ElementSet
        {
            SetLocationId = 1,
            ModuleId = 1,
            Place = 0,
            IsEditable = false,
            Styling = new SetStyling
            {
                is_appendix = false,
                is_horizontal = false,
                has_page_break = "false"
            },
            Elements = new List<ElementLocation>
            {
                new()
                {
                    SetLocationId = 1,
                    ElementId = 2,
                    Place = 0,
                    IsEditable = false
                },
                null
            }
        };

        var result = await this.repository?.Update(newElementSet)!;

        Assert.NotNull(result);
        Assert.AreEqual(newElementSet, result);

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
    public async Task UpdateInvalidNullLocationsElement()
    {
        var newElementSet = new ElementSet
        {
            SetLocationId = 1,
            ModuleId = 1,
            Place = 0,
            IsEditable = false,
            Styling = new SetStyling
            {
                is_appendix = false,
                is_horizontal = false,
                has_page_break = "false"
            }
        };

        var result = await this.repository?.Update(newElementSet)!;

        Assert.NotNull(result);
        Assert.AreEqual(newElementSet, result);

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
    public async Task UpdateElementLocationAttributes()
    {
        var elementLocation = new ElementLocation
        {
            SetLocationId = 1,
            ElementId = 1,
            Place = 1,
            IsEditable = false,
            Attributes = new SetLocationAttributes
            {
                Width = "500",
                Height = "200",
                Alignment = "left"
            }
        };
        var oldAlignment = "end";
        var oldHeight = "700";
        await this.repository.UpdateElementLocationAttributes(elementLocation);
        var actual = MockDataContext.Locations.Where(x => x.SetLocationId == elementLocation.SetLocationId)
            .ElementAt(1);
        Assert.AreEqual(elementLocation.SetLocationId, actual.SetLocationId);
        Assert.AreEqual(elementLocation.ElementId, actual.ElementId);
        Assert.AreEqual(elementLocation.Place, actual.Place);
        Assert.AreNotEqual(oldAlignment, actual.Attributes.Alignment);
        Assert.AreNotEqual(oldHeight, actual.Attributes.Height);
        Assert.AreEqual(elementLocation.Attributes.Width, actual.Attributes.Width);
    }

    [Test]
    public async Task UpdateElementLocationAttributesForAnchorAttributes()
    {
        var elementLocation = new ElementLocation
        {
            SetLocationId = 1,
            ElementId = 1,
            Place = 1,
            IsEditable = false,
            Attributes = new SetLocationAttributes
            {
                Width = "500",
                Height = "200",
                Alignment = "left",
                HeadingLevel = 1
            }
        };

        await this.repository.UpdateElementLocationAttributes(elementLocation);
        var actual = MockDataContext.Locations.Where(x => x.SetLocationId == elementLocation.SetLocationId)
            .ElementAt(1);
        Assert.AreEqual(elementLocation.SetLocationId, actual.SetLocationId);
        Assert.AreEqual(elementLocation.ElementId, actual.ElementId);
        Assert.AreEqual(elementLocation.Place, actual.Place);
        Assert.AreEqual(elementLocation.Attributes.Width, actual.Attributes.Width);
        Assert.AreNotEqual(0, actual.Attributes.HeadingLevel);
    }

    #endregion
}