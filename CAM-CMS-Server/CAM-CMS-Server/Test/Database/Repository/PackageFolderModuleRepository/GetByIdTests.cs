using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Context;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.PackageFolderModuleRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetByIdTests
{
    #region Data members

    private IDataContext context;

    private IPackageFolderModuleRepository repository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        this.context = new MockDataContext();

        this.repository = new CAMCMSServer.Database.Repository.PackageFolderModuleRepository(this.context);
    }

    #endregion

    //[Test]
    //public async Task GetAllByPackageId()
    //{
    //    var modules = await this.repository.SelectAllFromPackageId(3);

    //    var result = MockDataContext.PackageFolderModules.Where(x => x.PackageFolderId == 3);

    //    Assert.AreEqual(result, modules);
    //}
}