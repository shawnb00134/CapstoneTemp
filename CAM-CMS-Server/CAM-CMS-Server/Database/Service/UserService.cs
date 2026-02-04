//using Amazon.CognitoIdentityProvider;
//using Amazon.CognitoIdentityProvider.Model;

using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Model.Requests;

namespace CAMCMSServer.Database.Service;

public interface IUserService
{
    #region Methods

    Task<IEnumerable<User>> GetUsers();

    Task<IEnumerable<TempUser>> GetAllTempUsers();

    Task<User> GetUserById(int id);

    Task<IEnumerable<Model.Context>> GetAllContexts();

    Task<IEnumerable<AccessRole>> GetAllRoles();

    Task<IEnumerable<Privilege>> GetAllPrivileges();

    Task<User> CreateUserRoles(UserAccessRoleRequest request, int creatorId);

    Task<User> UpdateUserRoles(UserAccessRoleRequest request);

    Task<User> DeleteUserRoles(UserAccessRoleRequest request);

    Task<User?> ValidateUser(User user);

    Task<bool> ValidateReadRequest(int userId, int libraryId, int organizationId);

    Task<bool> ValidateCreateRequest(int userId, int libraryId, int organizationId);

    Task<bool> ValidateUpdateRequest(int userId, int libraryId, int organizationId);

    Task<bool> ValidateDeleteRequest(int userId, int libraryId, int organizationId);

    Task<bool> ValidateSystemRequest(int userId, string privilege);

    Task<bool> ValidateOrganizationRequest(int userId, int organizationId, string privilege);
    Task<bool> ValidateLibraryRequest(int userId, int libraryId, string privilege);

    Task<bool> UserIsAdmin(int userId);

    Task CreateTempUser(TempUser tempUser);

    Task<Invitation> CreateInvitation(Invitation invitation);

    Task UpdateUser(User appUser);

    Task UpdateTempUser(TempUser tempUser);

    Task CreateUser(User user);

    Task<bool> DeleteUser(int userId);

    Task<bool> DeleteTempUser(int tempUserId);

    //Task<AdminCreateUserResponse> RegisterUserWithCognito(TempUser user);
    Task<PrivilegeSummary?> CheckReadPrivileges(int userId);

    #endregion

    Task<bool> ValidatePackageRequest(int userId, int packageId, string request);
}

public class UserService : IUserService
{
    #region Constructors

    public UserService(IUserRepository repository, IInvitationRepository invitationRepository)
    {
        this.repository = repository;
        this.invitationRepository = invitationRepository;
    }

    #endregion

    #region Data members

    private readonly IUserRepository repository;
    private readonly IInvitationRepository invitationRepository;

    #endregion

    #region Methods

    public async Task<IEnumerable<User>> GetUsers()
    {
        var users = await repository.GetUsers();
        var roleTasks = new List<Task>();

        users.AsParallel().ForAll(user => roleTasks.Add(getRoles(user)));

        await Task.WhenAll(roleTasks);
        return users;
    }

    public async Task<IEnumerable<TempUser>> GetAllTempUsers()
    {
        return await repository.GetAllTempUsers();
    }

    public async Task<User> GetUserById(int id)
    {
        var user = await repository.GetUserById(id);

        user.DisplayRoles = await repository.GetUsersAccessRoles(user);

        return user;
    }

    public async Task<IEnumerable<Model.Context>> GetAllContexts()
    {
        return await repository.GetAllContexts();
    }

    public async Task<IEnumerable<AccessRole>> GetAllRoles()
    {
        var roles = await repository.GetAllRoles();
        return roles;
    }

    public async Task<IEnumerable<Privilege>> GetAllPrivileges()
    {
        return await repository.GetAllPrivileges();
    }

    public async Task<User?> ValidateUser(User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        var dbUser = await repository.GetDbUser(user);

        if (dbUser == null)
        {
            await repository.CreateUser(user);
            dbUser = await repository.GetDbUser(user);
        }

        dbUser.DisplayRoles = await repository.GetUsersAccessRoles(dbUser);

        return dbUser;
    }

    public async Task<User> CreateUserRoles(UserAccessRoleRequest request, int creatorId)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        request.CreatedBy = creatorId;
        await repository.CreateUserRole(request);

        var user = await GetUserById(request.UserId);
        return user;
    }

    public async Task<User> UpdateUserRoles(UserAccessRoleRequest request)
    {
        await repository.UpdateUserRole(request);

        var user = await GetUserById(request.UserId);
        return user;
    }

    public async Task<User> DeleteUserRoles(UserAccessRoleRequest request)
    {
        await repository.DeleteUserRole(request);

        var user = await GetUserById(request.UserId);
        return user;
    }

    public async Task<bool> ValidateReadRequest(int userId, int libraryId, int organizationId)
    {
        requestPreconditions(userId, libraryId, organizationId);

        return await validateRequest(userId, libraryId, organizationId, "read");
    }

    public async Task<bool> ValidateCreateRequest(int userId, int libraryId, int organizationId)
    {
        requestPreconditions(userId, libraryId, organizationId);

        return await validateRequest(userId, libraryId, organizationId, "create");
    }

    public async Task<bool> ValidateUpdateRequest(int userId, int libraryId, int organizationId)
    {
        requestPreconditions(userId, libraryId, organizationId);

        return await validateRequest(userId, libraryId, organizationId, "update");
    }

    public async Task<bool> ValidateDeleteRequest(int userId, int libraryId, int organizationId)
    {
        requestPreconditions(userId, libraryId, organizationId);

        return await validateRequest(userId, libraryId, organizationId, "delete");
    }

    public async Task<bool> ValidateSystemRequest(int userId, string privilege)
    {
        var systemAuthRequest = new AuthorizationRequest
        {
            UserId = userId,
            ContextType = "system",
            PrivilegeName = privilege
        };

        return await repository.ValidateRequest(systemAuthRequest);
    }

    public async Task<bool> ValidateOrganizationRequest(int userId, int organizationId, string privilege)
    {
        var organizationAuthRequest = new AuthorizationRequest
        {
            UserId = userId,
            ContextInstance = organizationId,
            ContextType = "organization",
            PrivilegeName = privilege
        };

        return await repository.ValidateRequest(organizationAuthRequest);
    }

    public async Task<bool> ValidateLibraryRequest(int userId, int libraryId, string privilege)
    {
        var libraryAuthRequest = new AuthorizationRequest
        {
            UserId = userId,
            ContextInstance = libraryId,
            ContextType = "library folder",
            PrivilegeName = privilege
        };

        return await repository.ValidateRequest(libraryAuthRequest);
    }

    public async Task<bool> UserIsAdmin(int userId)
    {
        return await repository.UserIsAdmin(userId);
    }

    public async Task<PrivilegeSummary?> CheckReadPrivileges(int userId)
    {
        return await repository.CheckReadPrivileges(userId);
    }

    public async Task<bool> ValidatePackageRequest(int userId, int packageId, string request)
    {
        if (await this.UserIsAdmin(userId))
        {
            return true;
        }
        var packageAuthRequest = new AuthorizationRequest
        {
            UserId = userId,
            ContextInstance = packageId,
            ContextType = "package",
            PrivilegeName = request
        };

        return await this.repository.ValidateRequest(packageAuthRequest);
    }

    private async Task getRoles(User user)
    {
        user.DisplayRoles = await repository.GetUsersAccessRoles(user);
    }

    private async Task<bool> validateRequest(int userId, int libraryId, int organizationId, string privilegeName)
    {
        Task<bool> systemAuthTask = null;
        Task<bool> organizationAuthTask = null;
        Task<bool> libraryAuthTask = null;

        Parallel.Invoke(
            () => { systemAuthTask = ValidateSystemRequest(userId, privilegeName); },
            () => { organizationAuthTask = ValidateOrganizationRequest(userId, organizationId, privilegeName); },
            () => { libraryAuthTask = ValidateLibraryRequest(userId, libraryId, privilegeName); }
        );

        var authorization = await Task.WhenAll(systemAuthTask, organizationAuthTask, libraryAuthTask);

        return authorization.Any(x => x);
    }

    private static void requestPreconditions(int userId, int libraryId, int organizationId)
    {
        if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId));

        if (libraryId <= 0) throw new ArgumentOutOfRangeException(nameof(libraryId));

        if (organizationId <= 0) throw new ArgumentOutOfRangeException(nameof(organizationId));
    }

    public async Task CreateTempUser(TempUser tempUser)
    {
        await repository.CreateTempUser(tempUser);
    }

    public async Task<Invitation> CreateInvitation(Invitation invitation)
    {
        invitation.CreatedAt = DateTime.UtcNow;
        await invitationRepository.Add(invitation);
        return invitation;
    }

    public async Task UpdateUser(User appUser)
    {
        await repository.UpdateUser(appUser);
    }

    public async Task UpdateTempUser(TempUser tempUser)
    {
        await repository.UpdateTempUser(tempUser);
    }

    public async Task CreateUser(User user)
    {
        await repository.CreateUser(user);
    }

    public async Task<bool> DeleteUser(int userId)
    {
        var user = await repository.GetUserById(userId);
        if (user == null) return false;

        await repository.SoftDeleteUser(userId);
        return true;
    }

    public async Task<bool> DeleteTempUser(int tempUserId)
    {
        var tempUser = await repository.GetTempUserById(tempUserId);
        if (tempUser == null) return false;

        await repository.SoftDeleteTempUser(tempUserId);
        return true;
    }

    //public async Task<AdminCreateUserResponse> RegisterUserWithCognito(TempUser user)
    //{
    //    var provider = new AmazonCognitoIdentityProviderClient();
    //    var request = new AdminCreateUserRequest
    //    {
    //        UserPoolId = "us-east-1_UXMm4Is3s",
    //        Username = user.Email,
    //        UserAttributes = new List<AttributeType>
    //        {
    //            new() { Name = "email", Value = user.Email },
    //            new() { Name = "given_name", Value = user.Firstname },
    //            new() { Name = "family_name", Value = user.Lastname }
    //        },
    //        DesiredDeliveryMediums = new List<string> { "EMAIL" }
    //    };

    //    var response = await provider.AdminCreateUserAsync(request);
    //    return response;
    //}

    #endregion
}