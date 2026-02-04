using CAMCMSServer.Database.Mappers;

namespace CAMCMSServer.Model;

public class TempUser
{
    #region Properties

    [Column("temp_user_id")] public int? TempId { get; set; }

    [Column("app_user_id")] public int? AppUserId { get; set; }

    [Column("linked_app_user_id")] public int? LinkedAppUserId { get; set; }

    [Column("module_view_count")] public int? ModuleViewCount { get; set; }

    [Column("invitation_id")] public int? InvitationId { get; set; }

    public string? Timestamp { get; set; }

    [Column("username")] public string? Username { get; set; }

    [Column("firstname")] public string? Firstname { get; set; }

    [Column("lastname")] public string? Lastname { get; set; }

    [Column("phone")] public string? Phone { get; set; }

    [Column("is_deleted")] public bool IsDeleted { get; set; }

    [Column("email")] public string? Email { get; set; }

    [Column("cognito_id")] public string? CognitoId { get; set; }

    #endregion

    #region Methods

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;

        var other = (TempUser)obj;

        return TempId == other.TempId;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    #endregion
}