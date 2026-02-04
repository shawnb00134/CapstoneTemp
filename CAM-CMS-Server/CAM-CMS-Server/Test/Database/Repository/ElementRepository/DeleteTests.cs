using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.ElementRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class DeleteTests
{
    #region Data members

    private IElementRepository? repository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        this.repository = new CAMCMSServer.Database.Repository.ElementRepository(context);
    }

    [Test]
    public async Task RemoveOneValidElement()
    {
        var newElement = new Element { ElementId = 1 };

        var countBeforeAdd = MockDataContext.Elements.Count;

        await this.repository?.Delete(newElement)!; // TODO: Implement deleteLocations and checkDelete methods for this

        Assert.AreEqual(countBeforeAdd - 1, MockDataContext.Elements.Count);
        Assert.IsFalse(MockDataContext.Elements.Contains(newElement));
    }

    #endregion
}