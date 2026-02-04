using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;
//using Amazon.S3;

#pragma warning disable CS8618

namespace CAMCMSServer.Test.Database.Service.ModuleService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class UpdateTests
{
    #region Data members

    private IModuleService moduleService;

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
    public async Task UpdateOneValidElement()
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

        await this.moduleService.Update(newModule);

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

    #endregion
}