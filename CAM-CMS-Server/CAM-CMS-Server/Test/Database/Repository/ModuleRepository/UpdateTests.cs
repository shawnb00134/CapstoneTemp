using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8602

namespace CAMCMSServer.Test.Database.Repository.ModuleRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class UpdateTests
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
    public async Task ValidElementUpdate()
    {
        var newModule = new Module
        {
            ModuleId = 1,
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

        var result = await this.repository?.Update(newModule)!;

        Assert.NotNull(result);
        Assert.AreEqual(newModule, result);

        var actual = MockDataContext.Modules.Where(x => x.ModuleId == newModule.ModuleId).ElementAt(0);

        Assert.AreEqual(newModule.ModuleId, actual.ModuleId);
        Assert.AreEqual(newModule.Title, actual.Title);
        Assert.AreEqual(newModule.Description, actual.Description);
        Assert.AreEqual(newModule.SurveyStartLink, actual.SurveyStartLink);
        Assert.AreEqual(newModule.SurveyEndLink, actual.SurveyEndLink);
        Assert.AreEqual(newModule.ContactTitle, actual.ContactTitle);
        Assert.AreEqual(newModule.ContactNumber, actual.ContactNumber);
        Assert.AreEqual(newModule.Thumbnail, actual.Thumbnail);
        Assert.AreEqual(newModule.Tags, actual.Tags);
        Assert.AreEqual(newModule.DisplayTitle, actual.DisplayTitle);
        Assert.AreEqual(newModule.TemplateModuleId, actual.TemplateModuleId);
        Assert.AreEqual(newModule.IsTemplate, actual.IsTemplate);
        Assert.AreEqual(newModule.PublishedTime, actual.PublishedTime);
        Assert.AreEqual(newModule.CreatedAt, actual.CreatedAt);
        Assert.AreEqual(newModule.LibraryFolderId, actual.LibraryFolderId);
    }

    [Test]
    public async Task UpdateTwoValidElements()
    {
        var newModuleOne = new Module
        {
            ModuleId = 1,
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
            ModuleId = 2,
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

        var result = await this.repository?.Update(newModuleOne)!;

        Assert.NotNull(result);
        Assert.AreEqual(newModuleOne, result);

        result = await this.repository?.Update(newModuleTwo)!;

        Assert.NotNull(result);
        Assert.AreEqual(newModuleTwo, result);

        var actual = MockDataContext.Modules.Where(x => x.ModuleId == newModuleOne.ModuleId).ElementAt(0);

        Assert.AreEqual(newModuleOne.ModuleId, actual.ModuleId);
        Assert.AreEqual(newModuleOne.Title, actual.Title);
        Assert.AreEqual(newModuleOne.Description, actual.Description);
        Assert.AreEqual(newModuleOne.SurveyStartLink, actual.SurveyStartLink);
        Assert.AreEqual(newModuleOne.SurveyEndLink, actual.SurveyEndLink);
        Assert.AreEqual(newModuleOne.ContactTitle, actual.ContactTitle);
        Assert.AreEqual(newModuleOne.ContactNumber, actual.ContactNumber);
        Assert.AreEqual(newModuleOne.Thumbnail, actual.Thumbnail);
        Assert.AreEqual(newModuleOne.Tags, actual.Tags);
        Assert.AreEqual(newModuleOne.DisplayTitle, actual.DisplayTitle);
        Assert.AreEqual(newModuleOne.TemplateModuleId, actual.TemplateModuleId);
        Assert.AreEqual(newModuleOne.IsTemplate, actual.IsTemplate);
        Assert.AreEqual(newModuleOne.PublishedTime, actual.PublishedTime);
        Assert.AreEqual(newModuleOne.CreatedAt, actual.CreatedAt);
        Assert.AreEqual(newModuleOne.LibraryFolderId, actual.LibraryFolderId);

        actual = MockDataContext.Modules.Where(x => x.ModuleId == newModuleTwo.ModuleId).ElementAt(0);

        Assert.AreEqual(newModuleTwo.ModuleId, actual.ModuleId);
        Assert.AreEqual(newModuleTwo.Title, actual.Title);
        Assert.AreEqual(newModuleTwo.Description, actual.Description);
        Assert.AreEqual(newModuleTwo.SurveyStartLink, actual.SurveyStartLink);
        Assert.AreEqual(newModuleTwo.SurveyEndLink, actual.SurveyEndLink);
        Assert.AreEqual(newModuleTwo.ContactTitle, actual.ContactTitle);
        Assert.AreEqual(newModuleTwo.ContactNumber, actual.ContactNumber);
        Assert.AreEqual(newModuleTwo.Thumbnail, actual.Thumbnail);
        Assert.AreEqual(newModuleTwo.Tags, actual.Tags);
        Assert.AreEqual(newModuleTwo.DisplayTitle, actual.DisplayTitle);
        Assert.AreEqual(newModuleTwo.TemplateModuleId, actual.TemplateModuleId);
        Assert.AreEqual(newModuleTwo.IsTemplate, actual.IsTemplate);
        Assert.AreEqual(newModuleTwo.PublishedTime, actual.PublishedTime);
        Assert.AreEqual(newModuleTwo.CreatedAt, actual.CreatedAt);
        Assert.AreEqual(newModuleTwo.LibraryFolderId, actual.LibraryFolderId);
    }

    [Test]
    public void UpdateNullElement()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.Update(null!)!);
    }

    [Test]
    public void UpdateInvalidModuleIdElement()
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

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.Update(newModule));
    }

    [Test]
    public void UpdateInvalidTitleElement()
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

        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.Update(newModule));
    }

    [Test]
    public void UpdateInvalidDisplayTitleElement()
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

        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.Update(newModule));
    }

    [Test]
    public void UpdateInvalidIsTemplateElement()
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

        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.Update(newModule));
    }

    [Test]
    public void UpdateInvalidLibraryFolderIdElement()
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

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.Update(newModule));
    }

    #endregion
}