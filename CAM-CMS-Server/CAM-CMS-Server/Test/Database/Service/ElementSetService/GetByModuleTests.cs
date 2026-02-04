using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;
//using Amazon.S3;

namespace CAMCMSServer.Test.Database.Service.ElementSetService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetByModuleTests
{
    #region Data members

    private MockDataContext? context;
    private IElementService? elementService;
    private IElementSetService? elementSetService;

    #endregion

    #region Methods

    [SetUp]
    public void SetupEach()
    {
        this.context = new MockDataContext();

        var elementRepository = new ElementRepository(this.context);
        this.elementService = new CAMCMSServer.Database.Service.ElementService(elementRepository);

        var elementSetRepository = new ElementSetRepository(this.context);
        //this.elementSetService =
        //    new CAMCMSServer.Database.Service.ElementSetService(elementSetRepository, this.elementService,
        //        new FileService(new AmazonS3Client(), ""));
    }

    [Test]
    public async Task GetAllTest()
    {
        var sets = await this.elementSetService?.GetByModule(new Module { ModuleId = 1 })!;

        var expected = MockDataContext.Sets.Where(x => x.ModuleId == 1);
        foreach (var elementSet in expected)
        {
            elementSet.Elements = MockDataContext.Locations.Where(x => x.SetLocationId == elementSet.SetLocationId);
        }

        Assert.IsNotNull(sets);
        Assert.AreEqual(expected, sets);
    }

    #endregion
}