using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.ElementRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetAllTests
{
    #region Data members

    private MockDataContext? context;
    private IElementRepository? repository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        this.context = new MockDataContext();

        this.repository = new CAMCMSServer.Database.Repository.ElementRepository(this.context);
    }

    [Test]
    public async Task GetAllTest()
    {
        var elements = await this.repository?.GetAll()!;

        var expected = new List<Element>();
        MockDataContext.Elements.ForEach(element => expected.Add(element));

        Assert.IsNotNull(elements);
        Assert.AreEqual(expected, elements);
    }

    #endregion
}