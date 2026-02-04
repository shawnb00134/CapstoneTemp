using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.PackageRepositoy;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetByNameTests
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
    public void GetByNameTest()
    {
        var packageToFind = new Package
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
        };
        var package = this.repository.GetByName("Package 1").Result;

        Assert.AreEqual(packageToFind.Name, package.Name);
    }

    [Test]
    public void GetByNameNullTest()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository.GetByName(null));
    }

    #endregion
}