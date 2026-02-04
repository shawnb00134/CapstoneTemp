using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;
//using Amazon.S3;

#pragma warning disable CS8618

namespace CAMCMSServer.Test.Database.Service.ElementSetService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class DeleteTests
{
    #region Data members

    private IElementSetService elementSetService;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        var elementRepository = new ElementRepository(context);
        var elementService = new CAMCMSServer.Database.Service.ElementService(elementRepository);

        var elementSetRepository = new ElementSetRepository(context);
        //this.elementSetService = new CAMCMSServer.Database.Service.ElementSetService(elementSetRepository, elementService,
        //    new FileService(new AmazonS3Client(), ""));
    }

    [Test]
    public async Task RemoveOneValidElement()
    {
        var newElementSet = new ElementSet
        {
            SetLocationId = 1,
            Elements = new List<ElementLocation>()
        };

        var countBeforeAdd = MockDataContext.Sets.Count;

        await this.elementSetService.Delete(newElementSet);

        Assert.AreEqual(countBeforeAdd - 1, MockDataContext.Sets.Count);
        Assert.IsFalse(MockDataContext.Sets.Contains(newElementSet));
    }

    [Test]
    public async Task RemoveTwoValidElements()
    {
        var newElementSetOne = new ElementSet { SetLocationId = 1, Elements = new List<ElementLocation>() };
        var newElementSetTwo = new ElementSet { SetLocationId = 2, Elements = new List<ElementLocation>() };

        var countBeforeAdd = MockDataContext.Sets.Count;

        await this.elementSetService.Delete(newElementSetOne);
        await this.elementSetService.Delete(newElementSetTwo);

        Assert.AreEqual(countBeforeAdd - 2, MockDataContext.Sets.Count);
        Assert.IsFalse(MockDataContext.Sets.Contains(newElementSetOne));
        Assert.IsFalse(MockDataContext.Sets.Contains(newElementSetTwo));
    }

    #endregion
}