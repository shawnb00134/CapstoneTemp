using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.ElementService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CreateTests
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
    public async Task AddOneValidElement()
    {
        var newElement = new Element
        {
            ElementId = 1,
            Title = "Testing Element One",
            Description = "Testing Element One",
            TypeId = 1,
            Citation = "Testing Element One",
            Content = "{ \"test\": \"test\" }",
            ExternalSource = "SOURCE One",
            Tags = Array.Empty<string>(),
            LibraryFolderId = 1
        };

        var countBeforeAdd = MockDataContext.Elements.Count;

        var result = await this.elementService?.Create(newElement)!;

        Assert.NotNull(result);
        Assert.IsTrue(newElement.Equals(result));
        Assert.AreEqual(countBeforeAdd + 1, MockDataContext.Elements.Count);
        Assert.Contains(newElement, MockDataContext.Elements);
    }

    [Test]
    public async Task AddTwoValidElements()
    {
        var newElementOne = new Element
        {
            ElementId = 1,
            Title = "Testing Element One",
            Description = "Testing Element One",
            TypeId = 1,
            Citation = "Testing Element One",
            Content = "{ \"test\": \"test\" }",
            ExternalSource = "SOURCE One",
            Tags = Array.Empty<string>(),
            LibraryFolderId = 1
        };
        var newElementTwo = new Element
        {
            ElementId = 2,
            Title = "Testing Element Two",
            Description = "Testing Element Two",
            TypeId = 1,
            Citation = "Testing Element Two",
            Content = "{ \"test\": \"test\" }",
            ExternalSource = "SOURCE Two",
            Tags = Array.Empty<string>(),
            LibraryFolderId = 1
        };

        var countBeforeAdd = MockDataContext.Elements.Count;

        var result = await this.elementService?.Create(newElementOne)!;
        Assert.NotNull(result);
        Assert.IsTrue(newElementOne.Equals(result));

        result = await this.elementService?.Create(newElementTwo)!;
        Assert.NotNull(result);
        Assert.IsTrue(newElementTwo.Equals(result));

        Assert.AreEqual(countBeforeAdd + 2, MockDataContext.Elements.Count);
        Assert.Contains(newElementOne, MockDataContext.Elements);
        Assert.Contains(newElementTwo, MockDataContext.Elements);
    }

    [Test]
    public void AddNullElement()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.elementService?.Create(null!)!);
    }

    #endregion
}