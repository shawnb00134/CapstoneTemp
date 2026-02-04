using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.ElementService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetAllTests
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
        var elements = await this.elementService?.GetAll()!;

        var expected = new List<Element>();
        MockDataContext.Elements.ForEach(element => expected.Add(element));

        Assert.IsNotNull(elements);
        Assert.AreEqual(expected, elements);
    }

    #endregion
}