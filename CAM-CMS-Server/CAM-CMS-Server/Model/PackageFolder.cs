using CAMCMSServer.Database.Mappers;

namespace CAMCMSServer.Model;

public class PackageFolder
{
    #region Properties

    public IEnumerable<PackageFolder?>? PackageFolders { get; set; }

    public IEnumerable<PackageFolderModule?>? PackageFoldersModule { get; set; }

    [Column("package_folder_id")] public int PackageFolderId { get; set; }

    [Column("folder_type_id")] public int? FolderTypeId { get; set; }

    [Column("display_name")] public string? DisplayName { get; set; }

    [Column("full_description")] public string? FullDescription { get; set; }

    [Column("short_description")] public string? ShortDescription { get; set; }

    [Column("thumbnail")] public string? Thumbnail { get; set; }

    [Column("content_role_id")] public int? ContentRoleId { get; set; }

    [Column("package_id")] public int? PackageId { get; set; }

    [Column("published")] public bool? Published { get; set; }

    [Column("editable")] public bool? Editable { get; set; }

    [Column("parent_folder_id")] public int? ParentFolderId { get; set; }

    [Column("order_in_parent")] public int? OrderInParent { get; set; }

    [Column("created_at")] public string? CreatedAt { get; set; }

    [Column("created_by")] public int? CreatedBy { get; set; }

    [Column("updated_at")] public string? UpdatedAt { get; set; }

    [Column("updated_by")] public int? UpdatedBy { get; set; }

    [Column("published_at")] public string? PublishedAt { get; set; }

    [Column("is_deleted")] public bool? IsDeleted { get; set; }

    public string? Name { get; set; }

    #endregion

    #region Methods

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var other = (PackageFolder)obj;

        return this.PackageFolderId == other.PackageFolderId;
    }

    public override string ToString()
    {
        return
            $"PackageFolderId: {this.PackageFolderId}, PackageId: {this.PackageId}, ParentFolderId: {this.ParentFolderId}, OrderInParent: {this.OrderInParent}";
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    #endregion
}