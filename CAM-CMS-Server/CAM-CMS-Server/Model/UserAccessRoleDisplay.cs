namespace CAMCMSServer.Model;

public class UserAccessRoleDisplay
{
    #region Properties

    public int UserAccessRoleId { get; set; }

    public int AppUserId { get; set; }

    public int AccessRoleId { get; set; }

    public string Name { get; set; }

    public int ContextId { get; set; }

    public string Type { get; set; }

    public int? Instance { get; set; }

    #endregion
}