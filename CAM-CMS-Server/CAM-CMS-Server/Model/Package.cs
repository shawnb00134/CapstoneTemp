using CAMCMSServer.Database.Mappers;

namespace CAMCMSServer.Model;

public class Package
{
    #region Properties

    public IEnumerable<PackageFolder?>? PackageFolders { get; set; }

    [Column("package_id")] public int PackageId { get; set; }

    [Column("name")] public string? Name { get; set; }


    [Column("package_type_id")] public int? PackageTypeId { get; set; }

    [Column("is_core")] public bool? IsCore { get; set; }

    [Column("created_at")] public string? CreatedAt { get; set; }

    [Column("created_by")] public int? CreatedBy { get; set; }

    [Column("updated_at")] public string? UpdatedAt { get; set; }

    [Column("updated_by")] public int? UpdatedBy { get; set; }

    [Column("published_at")] public string? PublishedAt { get; set; }

    [Column("is_deleted")] public bool? IsDeleted { get; set; }

    #endregion

    #region Methods

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var other = (Package)obj;

        return this.PackageId == other.PackageId;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    #endregion
}