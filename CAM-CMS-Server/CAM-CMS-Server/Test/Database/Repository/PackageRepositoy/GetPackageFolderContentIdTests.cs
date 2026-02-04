using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.PackageRepositoy;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetPackageFolderContentIdTests
{
    #region Data members

    private IPackageRepository repository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();
        this.repository = new PackageRepository(context);
    }

    [Test]
    public void GetContentIdTest()
    {
        var packageFolder = new PackageFolder
        {
            PackageFolderId = 1,
            ContentRoleId = 1
        };

        var result = this.repository.GetPackageFolderContentIdByFolderId(packageFolder.PackageFolderId);

        Assert.AreEqual((int?)packageFolder.ContentRoleId, result.Result);
    }

    #endregion
}