using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.OrganizationPackageRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ConstructorTests
{
    #region Data members

    private IOrganizationPackageRepository organizationPackageRepository;

    #endregion

    #region Methods

    [SetUp]
    public void SetUp()
    {
        this.organizationPackageRepository =
            new CAMCMSServer.Database.Repository.OrganizationPackageRepository(new MockDataContext());
    }

    [Test]
    public void NotNullTest()
    {
        var organizationPackageRepository =
            new CAMCMSServer.Database.Repository.OrganizationPackageRepository(new MockDataContext());
        Assert.NotNull(organizationPackageRepository);
    }

    #endregion
}