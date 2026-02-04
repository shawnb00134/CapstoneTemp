using System.ComponentModel.DataAnnotations.Schema;

namespace CAMCMSServer.Model;

public class Organization
{
    #region Properties

    [Column("organization_id")] public int? OrganizationId { get; set; }

    [Column("name")] public string? Name { get; set; }

    [Column("created_at")] public string? CreatedAt { get; set; }

    [Column("updated_at")] public string? UpdatedAt { get; set; }

    [Column("is_active")] public bool? IsActive { get; set; }

    [Column("tags")] public string[] Tags { get; set; }

    [Column("library_folder_id")] public int? LibraryFolderId { get; set; }

#endregion

    #region Methods

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var other = (Organization)obj;

        return this.OrganizationId == other.OrganizationId;
    }

    public override int GetHashCode()
    {
        return this.OrganizationId.GetHashCode() + this.Name.GetHashCode();
    }

    #endregion
}