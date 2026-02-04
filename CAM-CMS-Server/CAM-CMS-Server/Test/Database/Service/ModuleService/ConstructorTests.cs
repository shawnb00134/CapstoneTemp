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
public class ConstructorTests
{
    #region Data members

    private IModuleRepository moduleRepository;
    private IElementSetService elementSetService;
    private IElementService elementService;
    private IPublishedModuleRepository publishedModuleRepository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        this.moduleRepository = new ModuleRepository(context);

        var elementRepository = new ElementRepository(context);
        this.elementService = new CAMCMSServer.Database.Service.ElementService(elementRepository);

        var elementSetRepository = new ElementSetRepository(context);
        //this.elementSetService =
        //    new CAMCMSServer.Database.Service.ElementSetService(elementSetRepository, this.elementService,
        //        new FileService(new AmazonS3Client(), ""));
        this.publishedModuleRepository = new PublishedModuleRepository(context);
    }

    [Test]
    public void NotNullTest()
    {
        //var elementRepo =
        //    new CAMCMSServer.Database.Service.ModuleService(this.moduleRepository, this.elementSetService,
        //        this.elementService, this.publishedModuleRepository, null);

        //Assert.IsNotNull(elementRepo);
    }

    #endregion
}