namespace CAMCMSServer.Model.Requests;

public class SetStyling
{
    #region Properties

    public bool? is_appendix { get; set; }

    public bool? is_horizontal { get; set; }

    public string? has_page_break { get; set; }

    public bool? is_collapsible { get; set; }

    #endregion
}