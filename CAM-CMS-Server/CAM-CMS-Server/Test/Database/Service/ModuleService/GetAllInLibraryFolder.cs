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
public class GetAllInLibraryFolder
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
    public async Task GetByValidId()
    {
        var folder = new LibraryFolder { LibraryFolderId = 1 };

        var elements = await this.moduleService?.GetAllInLibraryFolder(folder)!;

        var expected = MockDataContext.Modules.Where(x => x.LibraryFolderId == folder.LibraryFolderId);

        Assert.IsNotNull(elements);
        Assert.AreEqual(expected, elements);
    }

    [Test]
    public async Task GetByInvalidId()
    {
        var folder = new LibraryFolder { LibraryFolderId = 0 };

        var elements = await this.moduleService?.GetAllInLibraryFolder(folder)!;

        var expected = MockDataContext.Modules.Where(x => x.LibraryFolderId == folder.LibraryFolderId);

        Assert.IsNotNull(elements);
        Assert.AreEqual(expected, elements);
    }

    #endregion
}