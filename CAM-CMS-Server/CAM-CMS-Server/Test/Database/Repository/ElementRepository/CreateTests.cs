using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8602

namespace CAMCMSServer.Test.Database.Repository.ElementRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CreateTests
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
    public async Task AddOneValidElement()
    {
        var newElement = new Element
        {
            ElementId = 2,
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

        var result = await this.repository?.Create(newElement)!;

        Assert.NotNull(result);
        Assert.AreEqual(newElement, result);
        Assert.AreEqual(countBeforeAdd + 1, MockDataContext.Elements.Count);
        Assert.Contains(newElement, MockDataContext.Elements);
    }

    [Test]
    public async Task AddTwoValidElements()
    {
        var newElementOne = new Element
        {
            ElementId = 2,
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
            ElementId = 5,
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

        var result = await this.repository?.Create(newElementOne)!;
        Assert.NotNull(result);
        Assert.AreEqual(newElementOne, result);

        result = await this.repository?.Create(newElementTwo)!;
        Assert.NotNull(result);
        Assert.AreEqual(newElementTwo, result);

        Assert.AreEqual(countBeforeAdd + 2, MockDataContext.Elements.Count);
        Assert.Contains(newElementOne, MockDataContext.Elements);
        Assert.Contains(newElementTwo, MockDataContext.Elements);
    }

    [Test]
    public async Task AddInvalidCreatedAtElement()
    {
        var newElement = new Element
        {
            ElementId = 2,
            Title = "Testing Element One",
            Description = "Testing Element One",
            TypeId = 1,
            Citation = "Testing Element One",
            Content = "{ \"test\": \"test\" }",
            ExternalSource = "SOURCE One",
            Tags = Array.Empty<string>(),
            LibraryFolderId = 1,
            CreatedAt = "TESTING"
        };

        var countBeforeAdd = MockDataContext.Elements.Count;

        var result = await this.repository?.Create(newElement)!;

        Assert.NotNull(result);
        Assert.AreEqual(newElement, result);
        Assert.AreEqual(countBeforeAdd + 1, MockDataContext.Elements.Count);
        Assert.Contains(newElement, MockDataContext.Elements);
    }

    [Test]
    public void AddNullElement()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.Create(null!));
    }

    [Test]
    public void AddInvalidElementIdElement()
    {
        var newElement = new Element
        {
            ElementId = -1,
            Title = "Testing Element One",
            Description = "Testing Element One",
            TypeId = 1,
            Citation = "Testing Element One",
            Content = "{ \"test\": \"test\" }",
            ExternalSource = "SOURCE One",
            Tags = Array.Empty<string>(),
            LibraryFolderId = 1
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.Create(newElement));
    }

    [Test]
    public void AddInvalidTitleElement()
    {
        var newElement = new Element
        {
            ElementId = 1,
            Description = "Testing Element One",
            TypeId = 1,
            Citation = "Testing Element One",
            Content = "{ \"test\": \"test\" }",
            ExternalSource = "SOURCE One",
            Tags = Array.Empty<string>(),
            LibraryFolderId = 1
        };

        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.Create(newElement));
    }

    [Test]
    public void AddInvalidTypeIdElement()
    {
        var newElement = new Element
        {
            ElementId = 1,
            Title = "Testing Element One",
            Description = "Testing Element One",
            TypeId = 0,
            Citation = "Testing Element One",
            Content = "{ \"test\": \"test\" }",
            ExternalSource = "SOURCE One",
            Tags = Array.Empty<string>(),
            LibraryFolderId = 1
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.Create(newElement));
    }

    [Test]
    public void AddInvalidLibraryFolderIdElement()
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
            LibraryFolderId = 0
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.Create(newElement));
    }

    #endregion
}