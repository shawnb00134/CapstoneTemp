using System.Text.Json;

namespace CAMCMSServer.Model.ElementTypes;

public struct ImageElementPersistenceJson
{
    #region Properties

    public ElementType ElementType => ElementType.Image;
    public string? Key { get; set; }

    public bool? Link { get; set; }

    #endregion
}

public class ImageElement : IElementType // TODO: What properties are needed here?
{
    #region Properties

    public ElementType ElementType => ElementType.Image;

    public string? Key { get; set; }

    public string? Source { get; set; }

    public bool Link { get; set; }

    #endregion

    #region Methods

    public JsonDocument ConvertToJsonForPersistence()
    {
        var json = new ImageElementPersistenceJson
        {
            Key = this.Key,
            Link = false
        };

        return JsonSerializer.SerializeToDocument(json);
    }

    public JsonDocument ConvertToJsonForUI()
    {
        var json = JsonSerializer.SerializeToDocument(this);
        return json;
    }

    public void ConvertFromJsonForPersistence(JsonDocument json)
    {
        var inputElement = json.Deserialize<ImageElement>();

        this.Source = inputElement?.Source;
        this.Key = inputElement?.Key;
    }

    #endregion
}