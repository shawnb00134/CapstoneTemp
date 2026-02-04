using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model.Requests;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8618

namespace CAMCMSServer.Test.Database.Repository.UserRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ValidateRequestTests
{
    #region Data members

    private IUserRepository repository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        this.repository = new CAMCMSServer.Database.Repository.UserRepository(context);
    }

    [Test]
    public async Task ValidateValidLibraryFolderRequest()
    {
        var libraryRequest = new AuthorizationRequest
        {
            UserId = 1,
            ContextType = "library folder",
            ContextInstance = 3,
            PrivilegeName = "create"
        };

        Assert.True(await this.repository.ValidateRequest(libraryRequest));
    }

    [Test]
    public async Task ValidateValidOrganizationRequest()
    {
        var libraryRequest = new AuthorizationRequest
        {
            UserId = 1,
            ContextType = "organization",
            ContextInstance = 3,
            PrivilegeName = "create"
        };

        Assert.True(await this.repository.ValidateRequest(libraryRequest));
    }

    [Test]
    public async Task ValidateValidSystemRequest()
    {
        var libraryRequest = new AuthorizationRequest
        {
            UserId = 1,
            ContextType = "system",
            PrivilegeName = "create"
        };

        Assert.True(await this.repository.ValidateRequest(libraryRequest));
    }

    [Test]
    public async Task ValidateInvalidLibraryFolderRequest()
    {
        var libraryRequest = new AuthorizationRequest
        {
            UserId = 2,
            ContextType = "library folder",
            ContextInstance = 3,
            PrivilegeName = "create"
        };

        Assert.False(await this.repository.ValidateRequest(libraryRequest));
    }

    [Test]
    public async Task ValidateInvalidOrganizationRequest()
    {
        var libraryRequest = new AuthorizationRequest
        {
            UserId = 2,
            ContextType = "organization",
            ContextInstance = 3,
            PrivilegeName = "create"
        };

        Assert.False(await this.repository.ValidateRequest(libraryRequest));
    }

    [Test]
    public async Task ValidateInvalidSystemRequest()
    {
        var libraryRequest = new AuthorizationRequest
        {
            UserId = 2,
            ContextType = "system",
            PrivilegeName = "create"
        };

        Assert.False(await this.repository.ValidateRequest(libraryRequest));
    }

    #endregion
}