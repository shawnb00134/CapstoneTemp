using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;
//using Amazon.S3;

#pragma warning disable CS8602

namespace CAMCMSServer.Test.Database.Service.ModuleService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class UpdateLibraryFolderTests
{
    #region Data members

    private IModuleService? moduleService;
    private IElementSetService? elementSetService;

    #endregion

    #region Methods

    [SetUp]
    public void SetUp()
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
        //this.elementSetService = new CAMCMSServer.Database.Service.ElementSetService(elementSetRepository, elementService,
        //    new FileService(new AmazonS3Client(), ""));
    }

    [Test]
    public async Task UpdateValidModuleNoElementMoved()
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
            LibraryFolderId = 3
        };
        await this.moduleService?.UpdateLibraryFolder(newModule)!;

        var actual = MockDataContext.Modules.Where(x => x.ModuleId == newModule.ModuleId).ElementAt(0);

        Assert.AreEqual(newModule.LibraryFolderId, actual.LibraryFolderId);
    }

    [Test]
    public async Task UpdateValidModuleWithElementMoved()
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
        await this.moduleService?.UpdateLibraryFolderWithElements(newModule)!;

        var actualModule = MockDataContext.Modules.Where(x => x.ModuleId == newModule.ModuleId).ElementAt(0);

        var actualSet = this.elementSetService.GetByModule(actualModule).Result;

        var elementList = new List<Element>();

        foreach (var set in actualSet)
        {
            foreach (var element in set.Elements)
            {
                if (element.Element != null)
                {
                    elementList.Add(element.Element);
                }
            }
        }

        Assert.AreEqual(newModule.LibraryFolderId, actualModule.LibraryFolderId);
        Assert.AreEqual(newModule.LibraryFolderId, elementList[0].LibraryFolderId);
    }

    #endregion
}