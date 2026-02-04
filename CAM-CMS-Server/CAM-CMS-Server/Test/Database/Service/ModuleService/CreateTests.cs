using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;
//using Amazon.S3;

namespace CAMCMSServer.Test.Database.Service.ModuleService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CreateTests
{
    #region Data members

    private IModuleService? moduleService;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        var moduleRepository = new ModuleRepository(context);

        var elementRepository = new ElementRepository(context);
        var elementService = new CAMCMSServer.Database.Service.ElementService(elementRepository);

        var elementSetRepository = new ElementSetRepository(context);
        //var elementSetService = new CAMCMSServer.Database.Service.ElementSetService(elementSetRepository, elementService,
        //    new FileService(new AmazonS3Client(), ""));
        //var publishedModuleRepository = new PublishedModuleRepository(context);
        //this.moduleService =
        //    new CAMCMSServer.Database.Service.ModuleService(moduleRepository, elementSetService, elementService,
        //        publishedModuleRepository, null);
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

        await this.moduleService?.Create(newModule)!;

        Assert.AreEqual(countBeforeAdd + 1, MockDataContext.Modules.Count);
        Assert.Contains(newModule, MockDataContext.Modules);
    }

    [Test]
    public async Task AddInvalidDisplayTitleElement()
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

        var countBeforeAdd = MockDataContext.Modules.Count;

        await this.moduleService?.Create(newModule)!;

        Assert.AreEqual(countBeforeAdd + 1, MockDataContext.Modules.Count);
        Assert.Contains(newModule, MockDataContext.Modules);
    }

    [Test]
    public async Task AddInvalidIsTemplateElement()
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

        var countBeforeAdd = MockDataContext.Modules.Count;

        await this.moduleService?.Create(newModule)!;

        Assert.AreEqual(countBeforeAdd + 1, MockDataContext.Modules.Count);
        Assert.Contains(newModule, MockDataContext.Modules);
    }

    [Test]
    public async Task AddInvalidLibraryFolderElement()
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
            PublishedTime = "PUBLISHED Three"
        };

        var countBeforeAdd = MockDataContext.Modules.Count;

        await this.moduleService?.Create(newModule)!;

        Assert.AreEqual(countBeforeAdd + 1, MockDataContext.Modules.Count);
        Assert.Contains(newModule, MockDataContext.Modules);
    }

    [Test]
    public void AddNullElement()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.moduleService?.Create(null!)!);
    }

    #endregion
}