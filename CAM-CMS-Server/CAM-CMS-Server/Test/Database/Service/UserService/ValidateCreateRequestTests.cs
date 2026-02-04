using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8618

namespace CAMCMSServer.Test.Database.Service.UserService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ValidateCreateRequestTests
{
    #region Data members

    // Mock user "Testing" has read, create, update, and delete privileges on library folder 3, org 3, and system
    // Mock user "Tester" has no privileges

    private IUserService service;

    private IInvitationRepository invitationRepository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        var repository = new UserRepository(context);

        this.service = new CAMCMSServer.Database.Service.UserService(repository, this.invitationRepository);
    }

    [Test]
    public async Task ValidateValidLibraryFolderRequest()
    {
        Assert.True(await this.service.ValidateCreateRequest(1, 3, 1));
    }

    [Test]
    public async Task ValidateValidOrganizationRequest()
    {
        Assert.True(await this.service.ValidateCreateRequest(1, 1, 3));
    }

    [Test]
    public async Task ValidateValidSystemRequest()
    {
        Assert.True(await this.service.ValidateCreateRequest(1, 1, 1));
    }

    [Test]
    public async Task ValidateInvalidLibraryFolderRequest()
    {
        Assert.False(await this.service.ValidateCreateRequest(2, 3, 1));
    }

    [Test]
    public async Task ValidateInvalidOrganizationRequest()
    {
        Assert.False(await this.service.ValidateCreateRequest(2, 1, 3));
    }

    [Test]
    public async Task ValidateInvalidSystemRequest()
    {
        Assert.False(await this.service.ValidateCreateRequest(2, 1, 1));
    }

    [Test]
    public void ValidateInvalidUserId()
    {
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await this.service.ValidateCreateRequest(0, 1, 1));
    }

    [Test]
    public void ValidateInvalidLibraryId()
    {
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await this.service.ValidateCreateRequest(1, 0, 1));
    }

    [Test]
    public void ValidateInvalidOrganizationId()
    {
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await this.service.ValidateCreateRequest(1, 1, 0));
    }

    #endregion
}