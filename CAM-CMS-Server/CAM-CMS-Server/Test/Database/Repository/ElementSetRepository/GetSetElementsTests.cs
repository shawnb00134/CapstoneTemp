using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8602

namespace CAMCMSServer.Test.Database.Repository.ElementSetRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetSetElementsTests
{
    #region Data members

    private MockDataContext? context;
    private IElementSetRepository? repository;

    #endregion

    #region Methods

    [SetUp]
    public void SetupEach()
    {
        this.context = new MockDataContext();

        this.repository = new CAMCMSServer.Database.Repository.ElementSetRepository(this.context);
    }

    [Test]
    public async Task GetAllTest()
    {
        var set = new ElementSet
        {
            SetLocationId = 1
        };

        var sets = await this.repository?.GetSetElements(set)!;

        var expected = MockDataContext.Locations.Where(x => x.SetLocationId == set.SetLocationId);

        Assert.IsNotNull(sets);
        Assert.AreEqual(expected, sets);
    }

    [Test]
    public void GetNullElements()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.GetSetElements(null!)!);
    }

    [Test]
    public void GetInvalidSetLocationIdElement()
    {
        var newElementSet = new ElementSet
        {
            SetLocationId = -1
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.GetSetElements(newElementSet));
    }

    #endregion
}