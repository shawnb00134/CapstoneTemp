using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;
//using Amazon.S3;

namespace CAMCMSServer.Test.Database.Service.ModuleService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetByModuleIdTests
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
    public async Task GetAllTestAsync()
    {
        var module = await this.moduleService?.GetByModuleId(1)!;

        var expected = MockDataContext.Modules.Where(x => x.ModuleId == 1).ElementAt(0);

        Assert.IsNotNull(module);
        Assert.AreEqual(expected, module);
    }

    [Test]
    public async Task GetAllAtBoundaryTestAsync()
    {
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => { await this.moduleService?.GetByModuleId(0)!; });
    }

    [Test]
    public async Task GetAllAboveBoundaryTestAsync()
    {
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => { await this.moduleService?.GetByModuleId(100)!; });
    }

    #endregion
}