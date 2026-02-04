using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.ElementSetRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class UpdateLocationTests
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
    public async Task UpdateValidElement()
    {
        var newElementLocation = new ElementLocation
        {
            SetLocationId = 1,
            ElementId = 2,
            IsEditable = false,
            Place = 0
        };

        await this.repository?.UpdateLocation(newElementLocation)!;

        var actual = MockDataContext.Locations.Where(x =>
                x.ElementId == newElementLocation.ElementId && x.SetLocationId == newElementLocation.SetLocationId)
            .ElementAt(0);

        Assert.AreEqual(newElementLocation.SetLocationId, actual.SetLocationId);
        Assert.AreEqual(newElementLocation.ElementId, actual.ElementId);
        Assert.AreEqual(newElementLocation.IsEditable, actual.IsEditable);
        Assert.AreEqual(newElementLocation.Place, actual.Place);
    }

    [Test]
    public void UpdateNullElement()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.CreateLocation(null!)!);
    }

    [Test]
    public void UpdateInvalidSetIdElement()
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
    public void UpdateInvalidElementIdElement()
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
    public void UpdateInvalidPlaceElement()
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
    public void UpdateInvalidIsEditableElement()
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