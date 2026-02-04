using CAMCMSServer.Database.Mappers;

namespace CAMCMSServer.Model.Requests;

public class AuthorizationRequest
{
    #region Properties

    [Column("user_role_id")] public int? UserId { get; set; }

    [Column("type")] public string? ContextType { get; set; }

    [Column("instance")] public int? ContextInstance { get; set; }

    [Column("name")] public string? PrivilegeName { get; set; }

    #endregion
}