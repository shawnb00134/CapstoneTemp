using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.ElementService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class DeleteTests
{
    #region Data members

    private IElementService? elementService;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        var elementRepository = new ElementRepository(context);
        this.elementService = new CAMCMSServer.Database.Service.ElementService(elementRepository);
    }

    [Test]
    public async Task RemoveOneValidElement()
    {
        var newElement = new Element { ElementId = 4 };

        var countBeforeAdd = MockDataContext.Elements.Count;

        await this.elementService
            ?.Delete(newElement)!;

        Assert.AreEqual(countBeforeAdd - 1, MockDataContext.Elements.Count);
        Assert.IsFalse(MockDataContext.Elements.Contains(newElement));
    }

    [Test]
    public async Task AttemptRemoveNonValidElement()
    {
        var newElement = new Element { ElementId = 1 };

        var countBeforeAdd = MockDataContext.Elements.Count;

        await this.elementService
            ?.Delete(newElement)!;

        Assert.AreEqual(countBeforeAdd, MockDataContext.Elements.Count);
        Assert.IsTrue(MockDataContext.Elements.Contains(newElement));
    }

    [Test]
    public async Task AttemptRemoveNullElement()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.elementService?.Delete(null));
    }

    #endregion
}