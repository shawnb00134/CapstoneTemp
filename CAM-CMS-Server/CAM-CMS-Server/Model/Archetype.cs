namespace Model;

/// <summary>
///     The Object representation of the DB archetype
/// </summary>
public class Archetype
{
    #region Properties

    /// <summary>
    ///     Gets or sets the archetype identifier.
    /// </summary>
    /// <value>
    ///     The archetype identifier.
    /// </value>
    public int ArchetypeId { get; set; }

    /// <summary>
    ///     Gets or sets the name.
    /// </summary>
    /// <value>
    ///     The name.
    /// </value>
    public string Name { get; set; }

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
        if (obj == null || GetType() != obj.GetType()) return false;

        var other = (Archetype)obj;

        return ArchetypeId == other.ArchetypeId;
    }

    /// <summary>
    ///     Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode()
    {
        return ArchetypeId.GetHashCode() + Name.GetHashCode();
    }

    #endregion
}