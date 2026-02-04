using System.Data;
using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Context;
using CAMCMSServer.Model;
using Moq;

namespace CAMCMSServer.Test.Database.Context;

[ExcludeFromCodeCoverage]
public class MockDataContext : IDataContext
{
    #region Data members

    private readonly IDbConnectionWrapper connectionWrapper;

    #endregion

    #region Properties

    public static List<LibraryFolder> LibraryFolders => MockConnectionWrapper.LibraryFolders;

    public static List<Element> Elements => MockConnectionWrapper.Elements;
    public static List<ElementLocation> Locations => MockConnectionWrapper.Locations;
    public static List<ElementSet> Sets => MockConnectionWrapper.Sets;
    public static List<Module> Modules => MockConnectionWrapper.Modules;

    public static List<Package> Packages => MockConnectionWrapper.Packages;

    public static List<PackageFolder> PackageFolders => MockConnectionWrapper.PackageFolders;

    public static List<PackageFolderModule> PackageFolderModules => MockConnectionWrapper.PackageFoldersModule;

    public static List<PublishedModule> PublishedModules => MockConnectionWrapper.PublishedModules;
    public static List<User> Users => MockConnectionWrapper.Users;

    public static List<CAMCMSServer.Model.Context> Contexts => MockConnectionWrapper.Contexts;

    public static List<AccessRole> Roles => MockConnectionWrapper.Roles;

    public static List<Privilege> Privileges => MockConnectionWrapper.Privileges;

    public static List<Organization> Organizations => MockConnectionWrapper.Organizations;

    public static List<OrganizationPackage> OrganizationPackages => MockConnectionWrapper.OrganizationPackages;

    public static List<ContentRole> ContentRoles => MockConnectionWrapper.ContentRoles;

    public static List<OrganizationContentRole> OrganizationContentRoles => MockConnectionWrapper.OrganizationContentRoles;

    #endregion

    #region Constructors

    public MockDataContext()
    {
        var connection = new Mock<IDbConnection>();
        this.connectionWrapper = new MockConnectionWrapper(connection.Object);
        this.connectionWrapper.Open();
    }

    #endregion

    #region Methods

    public Task<IDbConnectionWrapper> CreateConnection()
    {
        return Task.FromResult(this.connectionWrapper);
    }

    public void Init()
    {
    }

    #endregion
}