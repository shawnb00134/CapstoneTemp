using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.PackageService;

public class GetAllTests
{
    #region Methods

    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class CreateTests
    {
        #region Data members

        private IPackageService? packageService;

        #endregion

        #region Methods

        [SetUp]
        public void Setup()
        {
            var context = new MockDataContext();

            var packageRepository = new PackageRepository(context);

            var packageFolderModuleRepository = new PackageFolderModuleRepository(context);

            this.packageService =
                new CAMCMSServer.Database.Service.PackageService(packageRepository, packageFolderModuleRepository);
        }

        [Test]
        public async Task GetAllPackages()
        {
            var packages = await this.packageService.GetAll();

            Assert.IsNotNull(packages);
            Assert.AreEqual(MockDataContext.Packages, packages);
        }

        #endregion
    }

    #endregion
}