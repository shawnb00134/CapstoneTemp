using CAMCMSServer.Model;
using NUnit.Framework;

namespace CAMCMSServer.Test.Model;

public class StudioAsideTests
{
    #region Methods

    [Test]
    public void TestGetValues()
    {
        var asideItems = new StudioAside
        {
            LibraryFolders = new List<LibraryFolder>
            {
                new()
                {
                    LibraryFolderId = 1,
                    CreatedBy = null,
                    Description = "descript",
                    Elements = new List<Element>
                    {
                        new()
                        {
                            ElementId = 1,
                            Title = "Testing Element One",
                            Description = "Testing Element One",
                            TypeId = 1,
                            Citation = "Testing Element One",
                            Content = "{ \"test\": \"test\" }",
                            ExternalSource = "SOURCE One",
                            Tags = Array.Empty<string>(),
                            LibraryFolderId = 1
                        }
                    },
                    Modules = new List<Module>
                    {
                        new()
                        {
                            ModuleId = 1,
                            Title = "Testing Module One",
                            Description = "Testing Module One",
                            SurveyStartLink = "Testing Module One",
                            SurveyEndLink = "Testing Module One",
                            ContactTitle = "Testing Module One",
                            ContactNumber = "Testing Module One",
                            Thumbnail = "Testing Module One",
                            Tags = Array.Empty<string>(),
                            DisplayTitle = "Testing Module One",
                            TemplateModuleId = 0,
                            IsTemplate = false,
                            PublishedTime = "PUBLISHED One",
                            LibraryFolderId = 1
                        }
                    }
                }
            },
            Packages = new List<Package>
            {
                new()
                {
                    CreatedAt = null,
                    CreatedBy = 2,
                    IsCore = true,
                    Name = "Package 1",
                    PackageFolders = new List<PackageFolder>(),
                    PackageId = 1,
                    PackageTypeId = 1,
                    PublishedAt = null,
                    UpdatedAt = null,
                    UpdatedBy = null
                }
            }
        };

        Assert.IsNotNull(asideItems);
        Assert.IsNotEmpty(asideItems.LibraryFolders);
        Assert.IsNotEmpty(asideItems.LibraryFolders.ElementAt(0).Elements);
        Assert.IsNotEmpty(asideItems.LibraryFolders.ElementAt(0).Modules);
    }

    #endregion
}