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
public class DeleteTests
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
    public async Task RemoveOneValidElement()
    {
        var newModule = new Module { ModuleId = 1, CreatedAt = "delete" };

        var countBeforeAdd = MockDataContext.Modules.Count;

        await this.moduleService?.Delete(newModule)!;

        Assert.AreEqual(countBeforeAdd - 1, MockDataContext.Modules.Count);
        Assert.IsFalse(MockDataContext.Modules.Contains(newModule));
    }

    [Test]
    public async Task RemoveOneValidWithAPublishedModule()
    {
        var newModule = new Module { ModuleId = 1, CreatedAt = "delete" };
        var newPublishedModule = new CAMCMSServer.Model.PublishedModule { Id = newModule.ModuleId };

        var countBeforeRemoveModule = MockDataContext.Modules.Count;

        var countBeforeRemovePublishedModule = MockDataContext.PublishedModules.Count;

        await this.moduleService?.Delete(newModule)!;

        Assert.AreEqual(countBeforeRemoveModule - 1, MockDataContext.Modules.Count);
        Assert.AreEqual(countBeforeRemovePublishedModule - 1, MockDataContext.PublishedModules.Count);

        Assert.IsFalse(MockDataContext.Modules.Contains(newModule));
        Assert.IsFalse(MockDataContext.PublishedModules.Contains(newPublishedModule));
    }

    #endregion
}