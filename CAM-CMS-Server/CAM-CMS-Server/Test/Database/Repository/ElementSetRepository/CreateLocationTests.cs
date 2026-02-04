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
public class CreateLocationTests
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
        var newElementLocation = new ElementLocation
        {
            SetLocationId = 1,
            ElementId = 2,
            IsEditable = false,
            Place = 0,
            Attributes = new SetLocationAttributes
            {
                Alignment = "left",
                HeadingLevel = 0,
                Height = "auto",
                Width = "auto"
            }
        };

        var countBeforeAdd = MockDataContext.Locations.Count;

        await this.repository?.CreateLocation(newElementLocation)!;

        Assert.AreEqual(countBeforeAdd + 1, MockDataContext.Locations.Count);
        Assert.Contains(newElementLocation, MockDataContext.Locations);
    }

    [Test]
    public void AddNullElement()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.CreateLocation(null!)!);
    }

    [Test]
    public void AddInvalidSetIdElement()
    {
        var newElementSet = new ElementLocation
        {
            SetLocationId = -1,
            ElementId = 2,
            IsEditable = false,
            Place = 0
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.CreateLocation(newElementSet));
    }

    [Test]
    public void AddInvalidElementIdElement()
    {
        var newElementSet = new ElementLocation
        {
            SetLocationId = 1,
            ElementId = -1,
            IsEditable = false,
            Place = 0
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.CreateLocation(newElementSet));
    }

    [Test]
    public void AddInvalidPlaceElement()
    {
        var newElementSet = new ElementLocation
        {
            SetLocationId = 1,
            ElementId = 2,
            IsEditable = false,
            Place = -1
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.CreateLocation(newElementSet));
    }

    [Test]
    public void AddInvalidIsEditableElement()
    {
        var newElementSet = new ElementLocation
        {
            SetLocationId = 1,
            ElementId = 2,
            Place = 0
        };

        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.CreateLocation(newElementSet));
    }

    #endregion
}