using CAMCMSServer.Database.Mappers;

namespace CAMCMSServer.Model;

public class AccessRole
{
    #region Properties

    [Column("access_role_id")] public int Id { get; set; }

    [Column("name")] public string Name { get; set; }

    public IEnumerable<Privilege>? Privileges { get; set; }

    #endregion
}