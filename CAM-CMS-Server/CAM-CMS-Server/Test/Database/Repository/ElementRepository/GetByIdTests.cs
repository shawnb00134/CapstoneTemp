using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8602

namespace CAMCMSServer.Test.Database.Repository.ElementRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetByIdTests
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
        var element = await this.repository?.GetById(1, 1)!;

        var elementsList = new List<Element>();
        MockDataContext.Elements.ForEach(elementRequest => elementsList.Add(elementRequest));
        var expected = elementsList.Where(x => x.ElementId == 1).ElementAt(0);

        Assert.IsNotNull(element);
        Assert.AreEqual(expected, element);
    }

    [Test]
    public async Task GetAllAtBoundaryTest()
    {
        var element = await this.repository?.GetById(0, 1)!;

        Assert.IsNull(element);
    }

    [Test]
    public void GetAllInvalidIdTest()
    {
        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.GetById(-1, 1));
    }

    #endregion
}