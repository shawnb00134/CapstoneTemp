using System.Text.Json;
using CAMCMSServer.Database.Mappers;
using CAMCMSServer.Model.Requests;

namespace CAMCMSServer.Model;

public class ElementSet
{
    #region Properties

    public IEnumerable<ElementLocation?>? Elements { get; set; }

    [Column("element_set_id")] public int? SetLocationId { get; set; }

    [Column("module_id")] public int? ModuleId { get; set; }

    [Column("order_in_module")] public int? Place { get; set; }

    [Column("editable")] public bool? IsEditable { get; set; }

    [Column("set_attribute", "json")]
    public string? StylingJson
    {
        get
        {
            var stylingObject = this.Styling ?? new SetStyling
                { has_page_break = "false", is_appendix = false, is_horizontal = false, is_collapsible = false };
            return JsonSerializer.Serialize(stylingObject);
        }
        set => this.Styling = JsonSerializer.Deserialize<SetStyling>(value ?? "{}");
    }

    public SetStyling? Styling { get; set; }

    #endregion

    #region Methods

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var other = (ElementSet)obj;

        return this.SetLocationId == other.SetLocationId;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    #endregion
}