using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8602

namespace CAMCMSServer.Test.Database.Repository.ElementRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class UpdateTests
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
    public async Task UpdateOneValidElement()
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

        var result = await this.repository?.Update(newElement)!;

        Assert.NotNull(result);
        Assert.AreEqual(newElement, result);

        var actual = MockDataContext.Elements.Where(x => x.ElementId == newElement.ElementId).ElementAt(0);

        Assert.AreEqual(newElement.ElementId, actual.ElementId);
        Assert.AreEqual(newElement.Title, actual.Title);
        Assert.AreEqual(newElement.Description, actual.Description);
        Assert.AreEqual(newElement.TypeId, actual.TypeId);
        Assert.AreEqual(newElement.Citation, actual.Citation);
        Assert.AreEqual(newElement.ExternalSource, actual.ExternalSource);
        Assert.AreEqual(newElement.Tags, actual.Tags);
        Assert.AreEqual(newElement.CreatedAt, actual.CreatedAt);
        Assert.AreEqual(newElement.LibraryFolderId, actual.LibraryFolderId);
    }

    [Test]
    public async Task UpdateTwoValidElements()
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
            ElementId = 3,
            Title = "Testing Element Two",
            Description = "Testing Element Two",
            TypeId = 1,
            Citation = "Testing Element Two",
            Content = "{ \"test\": \"test\" }",
            ExternalSource = "SOURCE Two",
            Tags = Array.Empty<string>(),
            LibraryFolderId = 1
        };

        var result = await this.repository?.Update(newElementOne)!;

        Assert.NotNull(result);
        Assert.AreEqual(newElementOne, result);

        result = await this.repository?.Update(newElementTwo)!;

        Assert.NotNull(result);
        Assert.AreEqual(newElementTwo, result);

        var actualOne = MockDataContext.Elements.Where(x => x.ElementId == newElementOne.ElementId).ElementAt(0);
        var actualTwo = MockDataContext.Elements.Where(x => x.ElementId == newElementTwo.ElementId).ElementAt(0);

        Assert.AreEqual(newElementOne.ElementId, actualOne.ElementId);
        Assert.AreEqual(newElementOne.Title, actualOne.Title);
        Assert.AreEqual(newElementOne.Description, actualOne.Description);
        Assert.AreEqual(newElementOne.TypeId, actualOne.TypeId);
        Assert.AreEqual(newElementOne.Citation, actualOne.Citation);
        Assert.AreEqual(newElementOne.ExternalSource, actualOne.ExternalSource);
        Assert.AreEqual(newElementOne.Tags, actualOne.Tags);
        Assert.AreEqual(newElementOne.CreatedAt, actualOne.CreatedAt);
        Assert.AreEqual(newElementOne.LibraryFolderId, actualOne.LibraryFolderId);

        Assert.AreEqual(newElementTwo.ElementId, actualTwo.ElementId);
        Assert.AreEqual(newElementTwo.Title, actualTwo.Title);
        Assert.AreEqual(newElementTwo.Description, actualTwo.Description);
        Assert.AreEqual(newElementTwo.TypeId, actualTwo.TypeId);
        Assert.AreEqual(newElementTwo.Citation, actualTwo.Citation);
        Assert.AreEqual(newElementTwo.ExternalSource, actualTwo.ExternalSource);
        Assert.AreEqual(newElementTwo.Tags, actualTwo.Tags);
        Assert.AreEqual(newElementTwo.CreatedAt, actualTwo.CreatedAt);
        Assert.AreEqual(newElementTwo.LibraryFolderId, actualTwo.LibraryFolderId);
    }

    [Test]
    public void UpdateNullElement()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.Update(null!)!);
    }

    [Test]
    public void UpdateInvalidElementIdElement()
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

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.Update(newElement));
    }

    [Test]
    public void UpdateInvalidTitleElement()
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

        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.Update(newElement));
    }

    [Test]
    public void UpdateInvalidTypeIdElement()
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

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.Update(newElement));
    }

    [Test]
    public void UpdateInvalidLibraryFolderIdElement()
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

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.Update(newElement));
    }

    [Test]
    public void UpdateAnchorElement()
    {
        var anchorElement = new Element
        {
            ElementId = 4,
            Title = "Anchor",
            Description = "",
            TypeId = 7,
            Citation = "",
            Content = "",
            ExternalSource = "",
            Tags = Array.Empty<string>(),
            LibraryFolderId = 1
        };
        var oldTitle = "Test Anchor";
        this.repository?.Update(anchorElement);
        var actual = MockDataContext.Elements.Where(x => x.ElementId == anchorElement.ElementId).ElementAt(0);
        Assert.AreEqual(actual.ElementId, anchorElement.ElementId);
        Assert.AreNotEqual(oldTitle, actual.Title);
    }

    #endregion
}