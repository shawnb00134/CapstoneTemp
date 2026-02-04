using System.Text.Json.Serialization;
using CAMCMSServer.Utils;

namespace CAMCMSServer.Model.Requests;

public class SetLocationAttributes
{
    #region Properties

    [JsonConverter(typeof(StringOrNumberConverter))]
    public string? Width { get; set; }

    [JsonConverter(typeof(StringOrNumberConverter))]
    public string? Height { get; set; }

    public string? Alignment { get; set; }

    public int? HeadingLevel { get; set; }

    public bool? DisplayLink { get; set; }

    #endregion
}