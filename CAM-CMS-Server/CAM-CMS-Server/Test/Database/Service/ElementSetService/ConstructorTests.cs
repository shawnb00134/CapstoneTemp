using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;
//using Amazon.S3;

#pragma warning disable CS8618

namespace CAMCMSServer.Test.Database.Service.ElementSetService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ConstructorTests
{
    #region Data members

    private IElementSetRepository elementSetRepository;
    private IElementService elementService;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        this.elementSetRepository = new ElementSetRepository(context);

        var elementRepository = new ElementRepository(context);
        this.elementService = new CAMCMSServer.Database.Service.ElementService(elementRepository);
    }

    [Test]
    public void NotNullTest()
    {
        //var elementRepo = new CAMCMSServer.Database.Service.ElementSetService(this.elementSetRepository, this.elementService,
        //    new FileService(new AmazonS3Client(), ""));

        //Assert.IsNotNull(elementRepo);
    }

    #endregion
}