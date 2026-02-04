using CAMCMSServer.Database.Mappers;

namespace CAMCMSServer.Model;

public class Context
{
    #region Properties

    [Column("context_id")] public int Id { get; set; }

    [Column("type")] public string Type { get; set; }

    [Column("instance")] public int? Instance { get; set; }

    [Column("name")] public string? InstanceName { get; set; }

    #endregion
}