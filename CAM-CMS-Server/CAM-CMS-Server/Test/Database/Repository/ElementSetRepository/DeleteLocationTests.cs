using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8602

namespace CAMCMSServer.Test.Database.Repository.ElementSetRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class DeleteLocationTests
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
    public async Task RemoveOneValidElement()
    {
        var newElementLocation = new ElementLocation { SetLocationId = 1, ElementId = 2 };

        var countBeforeDelete = MockDataContext.Locations.Count;

        await this.repository?.DeleteLocation(newElementLocation)!;

        Assert.AreEqual(countBeforeDelete - 1, MockDataContext.Locations.Count);
        Assert.IsFalse(MockDataContext.Locations.Contains(newElementLocation));
    }

    [Test]
    public void DeleteNullElement()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.DeleteLocation(null!)!);
    }

    [Test]
    public void DeleteInvalidSetLocationIdElement()
    {
        var newElementLocation = new ElementLocation
        {
            SetLocationId = -1
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.DeleteLocation(newElementLocation));
    }

    [Test]
    public void DeleteInvalidElementIdElement()
    {
        var newElementLocation = new ElementLocation
        {
            SetLocationId = 1,
            ElementId = -1
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.DeleteLocation(newElementLocation));
    }

    #endregion
}