using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;
//using Amazon.S3;

namespace CAMCMSServer.Test.Database.Service.PublishedModule.cs;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ConstructorTests
{
    #region Data members

    private IPublishedModuleRepository publishedModuleRepository;

    private IElementRepository elementRepository;

    //private IFileService fileService;

    private readonly ILogger<PublishedModuleService> logger;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        this.publishedModuleRepository = new PublishedModuleRepository(context);

        this.elementRepository = new ElementRepository(context);

        //this.fileService = new FileService(new AmazonS3Client(), "");
    }

    [Test]
    public void NotNullTest()
    {
        //var moduleRepo = new PublishedModuleService(this.publishedModuleRepository, this.elementRepository,
        //    this.fileService, this.logger);
        //Assert.IsNotNull(moduleRepo);
    }

    #endregion
}