using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8602

namespace CAMCMSServer.Test.Database.Repository.ElementSetRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class DeleteTests
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
        var newElementSet = new ElementSet { SetLocationId = 1, Elements = new List<ElementLocation>() };

        var countBeforeDelete = MockDataContext.Sets.Count;

        await this.repository?.Delete(newElementSet)!;

        Assert.AreEqual(countBeforeDelete - 1, MockDataContext.Sets.Count);
        Assert.IsFalse(MockDataContext.Sets.Contains(newElementSet));
    }

    [Test]
    public void DeleteNullElement()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.Delete(null!)!);
    }

    [Test]
    public void DeleteInvalidSetLocationIdElement()
    {
        var newElementSet = new ElementSet
        {
            SetLocationId = -1
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.Delete(newElementSet));
    }

    #endregion
}