using CAMCMSServer.Database.Mappers;

namespace CAMCMSServer.Model.Requests;

public class UserAccessRoleRequest
{
    #region Properties

    [Column("app_user_id")] public int UserId { get; set; }

    public int ContextId { get; set; }

    public int AccessRoleId { get; set; }

    [Column("created_by")] public int? CreatedBy { get; set; }

    #endregion
}