using System.Text.Json;

namespace CAMCMSServer.Model.ElementTypes;

public struct PdfElementPersistenceJson
{
    #region Properties

    public ElementType ElementType => ElementType.Pdf;
    public string? Key { get; set; }

    #endregion
}

public class PdfElement : IElementType // TODO: What properties are needed here?
{
    #region Properties

    public ElementType ElementType => ElementType.Pdf;
    public bool Link { get; set; }

    public string? Source { get; set; }

    public string? Key { get; set; }

    #endregion

    #region Methods

    public JsonDocument ConvertToJsonForPersistence()
    {
        var json = new PdfElementPersistenceJson
        {
            Key = this.Key
        };

        return JsonSerializer.SerializeToDocument(json);
    }

    public JsonDocument ConvertToJsonForUI()
    {
        this.Key = null;
        var json = JsonSerializer.SerializeToDocument(this);
        return json;
    }

    public void ConvertFromJsonForPersistence(JsonDocument json)
    {
        var inputElement = json.Deserialize<PdfElement>();

        this.Source = inputElement?.Source;
        this.Key = inputElement?.Key;
    }

    #endregion
}