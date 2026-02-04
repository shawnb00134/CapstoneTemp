using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8602

namespace CAMCMSServer.Test.Database.Repository.ModuleRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CreateTests
{
    #region Data members

    private IModuleRepository? repository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        this.repository = new CAMCMSServer.Database.Repository.ModuleRepository(context);
    }

    [Test]
    public async Task AddOneValidElement()
    {
        var newModule = new Module
        {
            Title = "Testing Module Three",
            Description = "Testing Module Three",
            SurveyStartLink = "Testing Module Three",
            SurveyEndLink = "Testing Module Three",
            ContactTitle = "Testing Module Three",
            ContactNumber = "Testing Module Three",
            Thumbnail = "Testing Module Three",
            Tags = Array.Empty<string>(),
            DisplayTitle = "Testing Module Three",
            TemplateModuleId = 0,
            IsTemplate = false,
            PublishedTime = "PUBLISHED Three",
            LibraryFolderId = 1
        };

        var countBeforeAdd = MockDataContext.Modules.Count;

        var result = await this.repository?.Create(newModule)!;

        Assert.NotNull(result);
        Assert.AreEqual(newModule, result);
        Assert.AreEqual(countBeforeAdd + 1, MockDataContext.Modules.Count);
        Assert.Contains(newModule, MockDataContext.Modules);
    }

    [Test]
    public async Task AddValidElementToNonDefaultLibraryFolder()
    {
        var newModule = new Module
        {
            Title = "Testing Module Three",
            Description = "Testing Module Three",
            SurveyStartLink = "Testing Module Three",
            SurveyEndLink = "Testing Module Three",
            ContactTitle = "Testing Module Three",
            ContactNumber = "Testing Module Three",
            Thumbnail = "Testing Module Three",
            Tags = Array.Empty<string>(),
            DisplayTitle = "Testing Module Three",
            TemplateModuleId = 0,
            IsTemplate = false,
            PublishedTime = "PUBLISHED Three",
            LibraryFolderId = 3
        };
        var countBeforeAdd = MockDataContext.Modules.Count;
        var countOfModulesInFolderBefore = await this.repository?.GetByLibraryFolderId(3);
        var result = await this.repository?.Create(newModule);
        var countOfModulesInFolderAfter = await this.repository?.GetByLibraryFolderId(3);

        Assert.NotNull(result);
        Assert.AreEqual(newModule, result);
        Assert.AreEqual(countBeforeAdd + 1, MockDataContext.Modules.Count);
        Assert.AreEqual(countOfModulesInFolderBefore.Count() + 1, countOfModulesInFolderAfter.Count());
    }

    [Test]
    public async Task AddTwoValidElements()
    {
        var newModuleOne = new Module
        {
            Title = "Testing Module Three",
            Description = "Testing Module Three",
            SurveyStartLink = "Testing Module Three",
            SurveyEndLink = "Testing Module Three",
            ContactTitle = "Testing Module Three",
            ContactNumber = "Testing Module Three",
            Thumbnail = "Testing Module Three",
            Tags = Array.Empty<string>(),
            DisplayTitle = "Testing Module Three",
            TemplateModuleId = 0,
            IsTemplate = false,
            PublishedTime = "PUBLISHED Three",
            LibraryFolderId = 1
        };
        var newModuleTwo = new Module
        {
            Title = "Testing Module Four",
            Description = "Testing Module Four",
            SurveyStartLink = "Testing Module Four",
            SurveyEndLink = "Testing Module Four",
            ContactTitle = "Testing Module Four",
            ContactNumber = "Testing Module Four",
            Thumbnail = "Testing Module Four",
            Tags = Array.Empty<string>(),
            DisplayTitle = "Testing Module Four",
            TemplateModuleId = 0,
            IsTemplate = false,
            PublishedTime = "PUBLISHED Four",
            LibraryFolderId = 1
        };

        var countBeforeAdd = MockDataContext.Modules.Count;

        var resultOne = await this.repository?.Create(newModuleOne)!;
        var resultTwo = await this.repository?.Create(newModuleTwo)!;

        Assert.NotNull(resultOne);
        Assert.AreEqual(newModuleOne, resultOne);
        Assert.NotNull(resultTwo);
        Assert.AreEqual(newModuleTwo, resultTwo);
        Assert.AreEqual(countBeforeAdd + 2, MockDataContext.Modules.Count);
        Assert.Contains(newModuleOne, MockDataContext.Modules);
        Assert.Contains(newModuleTwo, MockDataContext.Modules);
    }

    [Test]
    public async Task AddOneInvalidCreatedAtElement()
    {
        var newModule = new Module
        {
            Title = "Testing Module Three",
            Description = "Testing Module Three",
            SurveyStartLink = "Testing Module Three",
            SurveyEndLink = "Testing Module Three",
            ContactTitle = "Testing Module Three",
            ContactNumber = "Testing Module Three",
            Thumbnail = "Testing Module Three",
            Tags = Array.Empty<string>(),
            DisplayTitle = "Testing Module Three",
            TemplateModuleId = 0,
            IsTemplate = false,
            PublishedTime = "PUBLISHED Three",
            LibraryFolderId = 1,
            CreatedAt = "TESTING"
        };

        var countBeforeAdd = MockDataContext.Modules.Count;

        var result = await this.repository?.Create(newModule)!;

        Assert.NotNull(result);
        Assert.AreEqual(newModule, result);
        Assert.AreEqual(countBeforeAdd + 1, MockDataContext.Modules.Count);
        Assert.Contains(newModule, MockDataContext.Modules);
    }

    [Test]
    public void AddNullElement()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.Create(null!)!);
    }

    [Test]
    public void AddInvalidModuleIdElement()
    {
        var newModule = new Module
        {
            ModuleId = -1,
            Title = "Testing Module Three",
            Description = "Testing Module Three",
            SurveyStartLink = "Testing Module Three",
            SurveyEndLink = "Testing Module Three",
            ContactTitle = "Testing Module Three",
            ContactNumber = "Testing Module Three",
            Thumbnail = "Testing Module Three",
            Tags = Array.Empty<string>(),
            DisplayTitle = "Testing Module Three",
            TemplateModuleId = 0,
            IsTemplate = false,
            PublishedTime = "PUBLISHED Three",
            LibraryFolderId = 1
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.Create(newModule));
    }

    [Test]
    public void AddInvalidTitleElement()
    {
        var newModule = new Module
        {
            Description = "Testing Module Three",
            SurveyStartLink = "Testing Module Three",
            SurveyEndLink = "Testing Module Three",
            ContactTitle = "Testing Module Three",
            ContactNumber = "Testing Module Three",
            Thumbnail = "Testing Module Three",
            Tags = Array.Empty<string>(),
            DisplayTitle = "Testing Module Three",
            TemplateModuleId = 0,
            IsTemplate = false,
            PublishedTime = "PUBLISHED Three",
            LibraryFolderId = 1
        };

        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.Create(newModule));
    }

    [Test]
    public void AddInvalidDisplayTitleElement()
    {
        var newModule = new Module
        {
            Title = "Testing Module Three",
            Description = "Testing Module Three",
            SurveyStartLink = "Testing Module Three",
            SurveyEndLink = "Testing Module Three",
            ContactTitle = "Testing Module Three",
            ContactNumber = "Testing Module Three",
            Thumbnail = "Testing Module Three",
            Tags = Array.Empty<string>(),
            TemplateModuleId = 0,
            IsTemplate = false,
            PublishedTime = "PUBLISHED Three",
            LibraryFolderId = 1
        };

        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.Create(newModule));
    }

    [Test]
    public void AddInvalidIsTemplateElement()
    {
        var newModule = new Module
        {
            Title = "Testing Module Three",
            Description = "Testing Module Three",
            SurveyStartLink = "Testing Module Three",
            SurveyEndLink = "Testing Module Three",
            ContactTitle = "Testing Module Three",
            ContactNumber = "Testing Module Three",
            Thumbnail = "Testing Module Three",
            Tags = Array.Empty<string>(),
            DisplayTitle = "Testing Module Three",
            TemplateModuleId = 0,
            PublishedTime = "PUBLISHED Three",
            LibraryFolderId = 1
        };

        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.Create(newModule));
    }

    [Test]
    public void AddInvalidLibraryFolderIdElement()
    {
        var newModule = new Module
        {
            Title = "Testing Module Three",
            Description = "Testing Module Three",
            SurveyStartLink = "Testing Module Three",
            SurveyEndLink = "Testing Module Three",
            ContactTitle = "Testing Module Three",
            ContactNumber = "Testing Module Three",
            Thumbnail = "Testing Module Three",
            Tags = Array.Empty<string>(),
            DisplayTitle = "Testing Module Three",
            TemplateModuleId = 0,
            IsTemplate = false,
            PublishedTime = "PUBLISHED Three",
            LibraryFolderId = 0
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.Create(newModule));
    }

    #endregion
}