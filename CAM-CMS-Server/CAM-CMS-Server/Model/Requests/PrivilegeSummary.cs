using CAMCMSServer.Database.Mappers;

namespace CAMCMSServer.Model.Requests;

public class PrivilegeSummary
{
    #region Properties

    [Column("app_user_id")] public int AppUserId { get; set; }

    [Column("access_role_id")] public int AccessRoleId { get; set; }

    [Column("library_read")] public bool LibraryRead { get; set; }

    [Column("organization_read")] public bool OrganizationRead { get; set; }

    [Column("system_read")] public bool SystemRead { get; set; }

    [Column("package_read")] public bool PackageRead { get; set; }

    #endregion
}