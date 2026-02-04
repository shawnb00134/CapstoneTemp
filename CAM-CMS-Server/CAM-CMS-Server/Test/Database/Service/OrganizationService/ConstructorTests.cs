using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.OrganizationService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ConstructorTests
{
    #region Data members

    #region Data Members

    private IOrganizationRepository organizationRepository;

    #endregion

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        this.organizationRepository = new OrganizationRepository(context);
    }

    [Test]
    public void NotNullTest()
    {
        var organizationService = new CAMCMSServer.Database.Service.OrganizationService(this.organizationRepository);

        Assert.IsNotNull(organizationService);
    }

    #endregion
}