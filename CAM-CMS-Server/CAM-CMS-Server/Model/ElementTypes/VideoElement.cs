using System.Text.Json;

namespace CAMCMSServer.Model.ElementTypes;

public class VideoElement : IElementType // TODO: What properties are needed here?
{
    #region Properties

    public ElementType ElementType => ElementType.Video;
    public bool Link { get; }

    public string? Source { get; set; }

    public string? Key { get; set; }

    #endregion

    #region Methods

    public JsonDocument ConvertToJsonForPersistence()
    {
        var json = JsonSerializer.SerializeToDocument(this);
        return json;
    }

    public JsonDocument ConvertToJsonForUI()
    {
        var json = JsonSerializer.SerializeToDocument(this);
        return json;
    }

    public void ConvertFromJsonForPersistence(JsonDocument json)
    {
        var inputElement = json.Deserialize<VideoElement>();

        this.Source = inputElement?.Source;
        this.Key = inputElement?.Key;
    }

    #endregion
}