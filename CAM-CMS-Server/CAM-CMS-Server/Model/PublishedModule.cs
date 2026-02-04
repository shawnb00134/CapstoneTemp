using CAMCMSServer.Database.Mappers;

namespace CAMCMSServer.Model;

public class PublishedModule
{
    #region Properties

    [Column("published_module_id")] public int Id { get; set; }

    [Column("cache", "json")] public string? Cache { get; set; }

    #endregion

    #region methods

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var other = obj as PublishedModule;
        return this.Id == other.Id;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    #endregion
}