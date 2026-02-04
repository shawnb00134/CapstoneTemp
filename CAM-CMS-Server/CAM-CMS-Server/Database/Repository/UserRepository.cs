using CAMCMSServer.Database.Context;
using CAMCMSServer.Model;
using CAMCMSServer.Model.Requests;
using CAMCMSServer.Utils;

namespace CAMCMSServer.Database.Repository;

public interface IUserRepository
{
    #region Methods

    Task<IEnumerable<User>> GetUsers();

    Task<User> GetUserById(int id);

    Task<User> GetUserByUsername(string username);

    Task<TempUser> GetTempUserById(int id);

    Task<User?> GetDbUser(User user);

    Task<IEnumerable<Model.Context>> GetAllContexts();

    Task<IEnumerable<AccessRole>> GetAllRoles();

    Task<IEnumerable<Privilege>> GetRolePrivileges(AccessRole role);

    Task<IEnumerable<Privilege>> GetAllPrivileges();

    Task<IEnumerable<UserAccessRoleDisplay>> GetUsersAccessRoles(User user);

    Task CreateUserRole(UserAccessRoleRequest userRole);

    Task UpdateUserRole(UserAccessRoleRequest userRole);

    Task DeleteUserRole(UserAccessRoleRequest userRole);

    Task<bool> ValidateRequest(AuthorizationRequest request);

    Task CreateUser(User user);
    Task<IDictionary<string, IDictionary<string, bool>>> GetPrivileges(int userId);
    Task<IDictionary<string, IDictionary<string, bool>>> GetPrivileges(int userId, int contextId);
    Task<IDictionary<string, bool>> GetUserReadPrivileges(int userId);
    Task<bool> UserIsAdmin(int userId);

    Task<PrivilegeSummary?> CheckReadPrivileges(int userId);

    Task UpdateUser(User user);

    Task DeleteUser(int userId);

    Task CreateTempUser(TempUser user);

    Task<IEnumerable<TempUser>> GetAllTempUsers();

    Task SoftDeleteUser(int userId);

    Task SoftDeleteTempUser(int tempUserId);
    Task UpdateTempUser(TempUser tempUser);

    #endregion
}

public class UserRepository : IUserRepository
{
    #region Data members

    private const int AdminRoleId = 2;

    private readonly IDataContext context;

    #endregion

    #region Constructors

    public UserRepository(IDataContext context)
    {
        this.context = context;
    }

    #endregion

    #region Methods

    public async Task<IEnumerable<User>> GetUsers()
    {
        using var connection = await this.context.CreateConnection();
        var users = await connection.QueryAsync<User>(SqlConstants.SelectAllUsers);
        return users;
    }
    public async Task<IEnumerable<TempUser>> GetAllTempUsers()
    {
        using var connection = await this.context.CreateConnection();
        var tempUsers = await connection.QueryAsync<TempUser>(SqlConstants.SelectAllTempUsers);
        return tempUsers;
    }

    public async Task<User> GetUserById(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        using var connection = await this.context.CreateConnection();
        var users = await connection.QueryAsync<User>(SqlConstants.SelectUser, new { Id = id });
        return users.FirstOrDefault();
    }

    public async Task<User> GetUserByUsername(string username)
    {
        using var connection = await this.context.CreateConnection();
        var users = await connection.QueryAsync<User>(SqlConstants.SelectUserByUsername, new { Username = username });
        return users.FirstOrDefault();
    }

    public async Task<TempUser> GetTempUserById(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        using var connection = await this.context.CreateConnection();
        var tempUsers = await connection.QueryAsync<TempUser>(SqlConstants.SelectTempUser, new { Id = id });
        return tempUsers.FirstOrDefault();
    }

    public async Task<User?> GetDbUser(User user)
    {
        using var connection = await this.context.CreateConnection();
        IEnumerable<User?> users = await connection.QueryAsync<User>(SqlConstants.SelectUser, user);

        var enumerable = users.ToList();
        return enumerable.Any() ? enumerable.ElementAt(0) : null;
    }

    public async Task<IEnumerable<Model.Context>> GetAllContexts()
    {
        using var connection = await this.context.CreateConnection();
        var contexts = await connection.QueryAsync<Model.Context>(SqlConstants.SelectAllContexts);
        return contexts;
    }

    public async Task<IEnumerable<AccessRole>> GetAllRoles()
    {
        using var connection = await this.context.CreateConnection();
        var roles = await connection.QueryAsync<AccessRole>(SqlConstants.SelectAllRoles);
        return roles;
    }

    public async Task<IEnumerable<Privilege>> GetAllPrivileges()
    {
        using var connection = await this.context.CreateConnection();
        var privileges = await connection.QueryAsync<Privilege>(SqlConstants.SelectAllPrivileges);
        return privileges;
    }

    public async Task<IEnumerable<Privilege>> GetRolePrivileges(AccessRole role)
    {
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        using var connection = await this.context.CreateConnection();
        var rolePrivileges = await connection.QueryAsync<Privilege>(SqlConstants.SelectRolePrivileges, role);
        return rolePrivileges;
    }

    public async Task<bool> ValidateRequest(AuthorizationRequest request)
    {
        using var connection = await this.context.CreateConnection();
        var userRoles = await this.GetUsersAccessRoles(await this.GetUserById((int)request.UserId!));
        if (await this.userIsAdmin(userRoles))
        {
            return true;
        }

        var sql = request.ContextType switch
        {
            "system" => SqlConstants.SelectPrivilegeSystem,
            "organization" => SqlConstants.SelectPrivilegeOrganization,
            "library folder" => SqlConstants.SelectPrivilegeLibrary,
            "package" => SqlConstants.SelectPrivilegePackage,
            _ => ""
        };

        var users = await connection.QueryAsync<AuthorizationRequest>(sql, request);
        return users.Any();
    }

    public async Task CreateUserRole(UserAccessRoleRequest userRole)
    {
        if (userRole == null)
        {
            throw new ArgumentNullException(nameof(userRole));
        }

        using var connection = await this.context.CreateConnection();
        await connection.ExecuteAsync(SqlConstants.InsertContext, userRole);
    }

    public async Task UpdateUserRole(UserAccessRoleRequest userRole)
    {
        if (userRole == null)
        {
            throw new ArgumentNullException(nameof(userRole));
        }

        using var connection = await this.context.CreateConnection();
        await connection.ExecuteAsync(SqlConstants.UpdateContext, userRole);
    }

    public async Task DeleteUserRole(UserAccessRoleRequest userRole)
    {
        if (userRole == null)
        {
            throw new ArgumentNullException(nameof(userRole));
        }

        using var connection = await this.context.CreateConnection();
        await connection.ExecuteAsync(SqlConstants.DeleteContext, userRole);
    }

    public async Task CreateUser(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        using var connection = await this.context.CreateConnection();
        await connection.ExecuteAsync(SqlConstants.InsertUser, user);
    }

    public async Task CreateTempUser(TempUser user)
    {
        ArgumentNullException.ThrowIfNull(user);

        using var connection = await this.context.CreateConnection();
        await connection.ExecuteAsync(SqlConstants.InsertTempUser, user);
    }

    public async Task<IDictionary<string, IDictionary<string, bool>>> GetPrivileges(int userId)
    {
        var privilegeDictionary = new Dictionary<string, IDictionary<string, bool>>();

        var contexts = await this.GetAllContexts();
        var user = await this.GetUserById(userId);

        foreach (var context in contexts)
        {
            var contextDictionary = this.createDefaultPrivilegeDictionary();
            var userRoles = (await this.GetUsersAccessRoles(user)).Where(request => request.ContextId == context.Id);

            var accessRolePrivileges = new List<Privilege>();

            foreach (var userRole in userRoles)
            {
                var role = await this.getRoleById(userRole.AccessRoleId);
                var rolePrivileges = await this.GetRolePrivileges(role);
                accessRolePrivileges.AddRange(rolePrivileges);
            }

            foreach (var privilege in accessRolePrivileges)
            {
                contextDictionary[privilege.Name] = true;
            }

            privilegeDictionary.Add(context.Type, contextDictionary);
        }

        return privilegeDictionary;
    }

    public async Task<IDictionary<string, IDictionary<string, bool>>> GetPrivileges(int userId, int contextId)
    {
        var privilegeDictionary = new Dictionary<string, IDictionary<string, bool>>();
        var contextDictionary = this.createDefaultPrivilegeDictionary();
        var context = await this.getContextById(contextId);

        var user = await this.GetUserById(userId);
        var userRoles = (await this.GetUsersAccessRoles(user)).Where(request => request.ContextId == context.Id);

        var accessRolePrivileges = new List<Privilege>();

        foreach (var userRole in userRoles)
        {
            var role = await this.getRoleById(userRole.AccessRoleId);
            var rolePrivileges = await this.GetRolePrivileges(role);
            accessRolePrivileges.AddRange(rolePrivileges);
        }

        foreach (var privilege in accessRolePrivileges)
        {
            contextDictionary[privilege.Name] = true;
        }

        privilegeDictionary.Add(context.Type, contextDictionary);
        return privilegeDictionary;
    }

    public async Task<IDictionary<string, bool>> GetUserReadPrivileges(int userId)
    {
        var readDictionary = this.createDefaultReadDictionary();

        var user = await this.GetUserById(userId);
        var userRoles = (await this.GetUsersAccessRoles(user)).ToList();
        var contexts = await this.GetAllContexts();

        if (await this.userIsAdmin(userRoles))
        {
            readDictionary["system"] = true;
            readDictionary["organization"] = true;
            readDictionary["library"] = true;
            return readDictionary;
        }

        foreach (var context in contexts)
        {
            var userRole = userRoles.FirstOrDefault(request => request.ContextId == context.Id);
            if (userRole == null)
            {
                continue;
            }

            switch (context.Type)
            {
                case "system":
                    readDictionary["system"] = true;
                    break;
                case "organization":
                    readDictionary["organization"] = true;
                    break;
                case "library":
                    readDictionary["library"] = true;
                    break;
            }
        }

        return readDictionary;
    }

    public async Task<bool> UserIsAdmin(int userId)
    {
        var userRoles = await this.GetUsersAccessRoles(await this.GetUserById(userId));

        return await this.userIsAdmin(userRoles);
    }

    public async Task<PrivilegeSummary?> CheckReadPrivileges(int userId)
    {
        using var connection = await this.context.CreateConnection();

        var readPrivileges =
            (await connection.QueryAsync<PrivilegeSummary>(SqlConstants.SelectUserReadPrivileges, new { UserId = userId }));

        var userReadPrivileges = readPrivileges.FirstOrDefault();

        if (userReadPrivileges is { AccessRoleId: AdminRoleId })
        {
            userReadPrivileges.LibraryRead = true;
            userReadPrivileges.OrganizationRead = true;
            userReadPrivileges.SystemRead = true;
            userReadPrivileges.PackageRead = true;
        }

        if (userReadPrivileges is not null)
        {
            return userReadPrivileges;
        }

        userReadPrivileges = new PrivilegeSummary
        {
            AccessRoleId = 0,
            AppUserId = userId,
            LibraryRead = false,
            OrganizationRead = false,
            SystemRead = false,
            PackageRead = false
        };

        return userReadPrivileges;
    }

    public async Task<IEnumerable<UserAccessRoleDisplay>> GetUsersAccessRoles(User user)
    {
        using var connection = await this.context.CreateConnection();

        var userRoles = await connection.QueryAsync<UserAccessRoleDisplay>(SqlConstants.SelectRolesForUser, user);
        return userRoles;
    }

    private async Task<bool> userIsAdmin(IEnumerable<UserAccessRoleDisplay> userRoles)
    {
        return userRoles.Any(request => request.AccessRoleId == AdminRoleId);
    }

    private async Task<Model.Context> getContextById(int contextId)
    {
        return (await this.GetAllContexts()).First(systemContext => systemContext.Id == contextId);
    }

    private IDictionary<string, bool> createDefaultReadDictionary()
    {
        return new Dictionary<string, bool>
        {
            { "system", false },
            { "organization", false },
            { "library", false }
        };
    }

    private IDictionary<string, bool> createDefaultPrivilegeDictionary()
    {
        return new Dictionary<string, bool>
        {
            { "read", false },
            { "create", false },
            { "delete", false },
            { "assign", false },
            { "invite", false },
            { "post", false },
            { "update", false }
        };
    }

    private async Task<AccessRole> getRoleById(int roleId)
    {
        using var connection = await this.context.CreateConnection();
        var roles = await connection.QueryAsync<AccessRole>(SqlConstants.SelectRoleById, new { Id = roleId });
        return roles.ElementAt(0);
    }


    public async Task UpdateUser(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        using var connection = await this.context.CreateConnection();
        await connection.ExecuteAsync(SqlConstants.UpdateUser, user);
    }

    public async Task UpdateTempUser(TempUser tempUser)
    {
        if (tempUser == null)
        {
            throw new ArgumentNullException(nameof(tempUser));
        }

        using var connection = await this.context.CreateConnection();
        await connection.ExecuteAsync(SqlConstants.UpdateTempUser, tempUser);
    }

    public async Task DeleteUser(int userId)
    {
        using var connection = await this.context.CreateConnection();
        await connection.ExecuteAsync(SqlConstants.DeleteUser, new { Id = userId });
    }

    public async Task SoftDeleteUser(int userId)
    {
        using var connection = await this.context.CreateConnection();
        await connection.ExecuteAsync(SqlConstants.SoftDeleteUser, new { Id = userId });
    }

    public async Task SoftDeleteTempUser(int tempUserId)
    {
        using var connection = await this.context.CreateConnection();
        await connection.ExecuteAsync(SqlConstants.SoftDeleteTempUser, new { Id = tempUserId });
    }

    #endregion
}
