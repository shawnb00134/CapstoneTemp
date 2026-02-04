using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8602

namespace CAMCMSServer.Test.Database.Repository.ElementSetRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetByIdTests
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
        var sets = await this.repository?.GetById(1, 1)!;

        var expected = MockDataContext.Sets
            .Where(x => x.SetLocationId == 1)
            .ElementAt(0);

        Assert.IsNotNull(sets);
        Assert.AreEqual(expected, sets);
    }

    [Test]
    public async Task GetAllTestAtBoundary()
    {
        var sets = await this.repository?.GetById(0, 0)!;

        Assert.IsNull(sets);
    }

    [Test]
    public void GetInvalidSetLocationIdElement()
    {
        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.GetById(-1, 1));
    }

    #endregion
}