using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8602

namespace CAMCMSServer.Test.Database.Repository.ElementRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetByValuesTests
{
    #region Data members

    private static readonly Element TestElement = new()
    {
        ElementId = 2,
        Title = "Testing Element One",
        Description = "Testing Element One",
        TypeId = 1,
        Citation = "Testing Element One",
        Content = "{ \"test\": \"test\" }",
        ExternalSource = "SOURCE One",
        Tags = Array.Empty<string>(),
        LibraryFolderId = 1
    };

    private MockDataContext? context;
    private IElementRepository? repository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        this.context = new MockDataContext();

        this.repository = new CAMCMSServer.Database.Repository.ElementRepository(this.context);
    }

    [Test]
    public async Task GetAllTest()
    {
        var element = await this.repository?.GetLastElement();

        var expectedRequest = MockDataContext.Elements.Where((x, i) => i == MockDataContext.Elements.Count - 1)
            .ElementAt(0);

        Assert.IsNotNull(element);
        Assert.AreEqual(expectedRequest, element);
    }

    #endregion
}