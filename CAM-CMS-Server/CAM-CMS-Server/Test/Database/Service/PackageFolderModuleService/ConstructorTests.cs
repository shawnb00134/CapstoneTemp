using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.PackageFolderModuleService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ConstructorTests
{
    #region Data members

    private IPackageFolderModuleRepository packageFolderModuleRepository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        this.packageFolderModuleRepository = new PackageFolderModuleRepository(context);
    }

    [Test]
    public void NotNullTest()
    {
        var packageFolderModuleService =
            new CAMCMSServer.Database.Service.PackageFolderModuleService(this.packageFolderModuleRepository);

        Assert.IsNotNull(packageFolderModuleService);
    }

    #endregion
}