using CAMCMSServer.Database.Mappers;
using CAMCMSServer.Model.Requests;

namespace CAMCMSServer.Model;

public class User
{
    #region Properties

    [Column("app_user_id")] public int? Id { get; set; }

    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }

    public string? Timestamp { get; set; }

    [Column("username")] public string? Username { get; set; }

    [Column("password")] public string? Password { get; set; }

    [Column("firstname")] public string? Firstname { get; set; }
    [Column("lastname")] public string? Lastname { get; set; }
    [Column("email")] public string? Email { get; set; }

    [Column("phone")] public string? Phone { get; set; }

    [Column("is_deleted")] public bool IsDeleted { get; set; }

    public IEnumerable<UserAccessRoleRequest>? Roles { get; set; }

    public IEnumerable<UserAccessRoleDisplay>? DisplayRoles { get; set; }

    #endregion

    #region Methods

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var other = (User)obj;

        return this.Id == other.Id;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    #endregion
}