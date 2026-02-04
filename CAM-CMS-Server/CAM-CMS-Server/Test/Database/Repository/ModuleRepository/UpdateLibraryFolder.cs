using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8602

namespace CAMCMSServer.Test.Database.Repository.ModuleRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class UpdateLibraryFolder
{
    #region Data members

    private IModuleRepository? repository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        this.repository = new CAMCMSServer.Database.Repository.ModuleRepository(context);
    }

    [Test]
    public async Task UpdateValidModule()
    {
        var newModule = new Module
        {
            ModuleId = 1,
            Title = "Testing Module Three",
            Description = "Testing Module Three",
            SurveyStartLink = "Testing Module Three",
            SurveyEndLink = "Testing Module Three",
            ContactTitle = "Testing Module Three",
            ContactNumber = "Testing Module Three",
            Thumbnail = "Testing Module Three",
            Tags = Array.Empty<string>(),
            DisplayTitle = "Testing Module Three",
            TemplateModuleId = 0,
            IsTemplate = false,
            PublishedTime = "PUBLISHED Three",
            LibraryFolderId = 1
        };
        await this.repository.Create(newModule);

        Assert.Contains(newModule, MockDataContext.Modules);

        newModule.LibraryFolderId = 4;

        var result = await this.repository.UpdateLibraryFolderId(newModule);

        Assert.AreEqual(4, result.LibraryFolderId);
    }

    [Test]
    public void UpdateNullModule()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.UpdateLibraryFolderId(null!)!);
    }

    [Test]
    public void UpdateInvalidModuleId()
    {
        var newModule = new Module
        {
            ModuleId = -1,
            Title = "Testing Module Three",
            Description = "Testing Module Three",
            SurveyStartLink = "Testing Module Three",
            SurveyEndLink = "Testing Module Three",
            ContactTitle = "Testing Module Three",
            ContactNumber = "Testing Module Three",
            Thumbnail = "Testing Module Three",
            Tags = Array.Empty<string>(),
            DisplayTitle = "Testing Module Three",
            TemplateModuleId = 0,
            IsTemplate = false,
            PublishedTime = "PUBLISHED Three",
            LibraryFolderId = 1
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.Update(newModule));
    }

    #endregion
}