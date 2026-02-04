using System.Text.Json;
using CAMCMSServer.Database.Mappers;
using CAMCMSServer.Model.Requests;

namespace CAMCMSServer.Model;

public class ElementLocation
{
    #region Properties

    public Element? Element { get; set; }

    [Column("element_set_id")] public int SetLocationId { get; set; }

    [Column("element_id")] public int ElementId { get; set; }

    [Column("order_in_set")] public int Place { get; set; }

    [Column("editable")] public bool? IsEditable { get; set; }

    [Column("location_attribute", "json")]
    public string? LocationAttributeJson
    {
        get
        {
            var locationAttributeObject = this.Attributes ?? new SetLocationAttributes
            {
                Width = "auto",
                Height = "auto",
                Alignment = "left",
                HeadingLevel = 0,
                DisplayLink = false
            };
            return JsonSerializer.Serialize(locationAttributeObject);
        }
        set
        {
            var attributes = JsonSerializer.Deserialize<SetLocationAttributes>(value ?? "{}");
            if (attributes != null)
            {
                attributes.HeadingLevel ??= 0;
            }
            this.Attributes = attributes;
        }
    }

    public SetLocationAttributes? Attributes { get; set; }

    #endregion

    #region Methods

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var other = (ElementLocation)obj;

        return this.SetLocationId == other.SetLocationId && this.ElementId == other.ElementId;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    #endregion
}