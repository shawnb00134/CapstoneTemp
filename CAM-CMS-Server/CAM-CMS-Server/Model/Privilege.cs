using CAMCMSServer.Database.Mappers;

namespace CAMCMSServer.Model;

public class Privilege
{
    #region Properties

    [Column("privilege_id")] public int Id { get; set; }

    [Column("name")] public string Name { get; set; }

    #endregion
}