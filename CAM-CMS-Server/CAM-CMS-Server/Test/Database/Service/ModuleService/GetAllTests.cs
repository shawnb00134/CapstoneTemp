using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;
//using Amazon.S3;

#pragma warning disable CS8618

namespace CAMCMSServer.Test.Database.Service.ModuleService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetAllTests
{
    #region Data members

    private MockDataContext? context;
    private IModuleService moduleService;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        this.context = new MockDataContext();

        var moduleRepository = new ModuleRepository(this.context);

        var elementRepository = new ElementRepository(this.context);
        var elementService = new CAMCMSServer.Database.Service.ElementService(elementRepository);

        var elementSetRepository = new ElementSetRepository(this.context);
        //var elementSetService = new CAMCMSServer.Database.Service.ElementSetService(elementSetRepository, elementService,
        //    new FileService(new AmazonS3Client(), ""));

        //var publishedModuleRepository = new PublishedModuleRepository(this.context);
        //this.moduleService =
        //    new CAMCMSServer.Database.Service.ModuleService(moduleRepository, elementSetService, elementService,
        //        publishedModuleRepository, null);
    }

    [Test]
    public async Task GetAllTest()
    {
        var modules = await this.moduleService.GetAll();

        Assert.IsNotNull(modules);
        Assert.AreEqual(MockDataContext.Modules, modules);

        foreach (var module in modules)
        {
            Assert.IsEmpty(module.ElementSets ?? throw new InvalidOperationException());
        }
    }

    #endregion
}