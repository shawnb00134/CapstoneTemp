using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.ElementService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetByIdTests
{
    #region Data members

    private MockDataContext? context;
    private IElementService? elementService;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        this.context = new MockDataContext();

        var elementRepository = new ElementRepository(this.context);
        this.elementService = new CAMCMSServer.Database.Service.ElementService(elementRepository);
    }

    [Test]
    public async Task GetAllTest()
    {
        var element = await this.elementService?.GetById(1)!;

        var elementsList = new List<Element>();
        MockDataContext.Elements.ForEach(elementRequest => elementsList.Add(elementRequest));
        var expected = elementsList.Where(x => x.ElementId == 1).ElementAt(0);

        Assert.IsNotNull(element);
        Assert.AreEqual(expected, element);
    }

    #endregion
}