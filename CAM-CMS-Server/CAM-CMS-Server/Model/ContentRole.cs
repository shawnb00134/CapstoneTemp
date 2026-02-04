namespace CAMCMSServer.Model;

/// <summary>
///     The Object representation of the DB content_role
/// </summary>
public class ContentRole
{
    #region Properties

    /// <summary>
    ///     Gets or sets the content role identifier.
    /// </summary>
    /// <value>
    ///     The content role identifier.
    /// </value>
    public int ContentRoleId { get; set; }

    /// <summary>
    ///     Gets or sets the name.
    /// </summary>
    /// <value>
    ///     The name.
    /// </value>
    public string Name { get; set; }

    public int ArchetypeId { get; set; }

    #endregion

    #region Methods

    /// <summary>
    ///     Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>
    ///     <see langword="true" /> if the specified object  is equal to the current object; otherwise,
    ///     <see langword="false" />.
    /// </returns>
    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var other = (ContentRole)obj;

        return this.ContentRoleId == other.ContentRoleId;
    }

    /// <summary>
    ///     Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode()
    {
        return this.ContentRoleId.GetHashCode() + this.Name.GetHashCode();
    }

    #endregion
}