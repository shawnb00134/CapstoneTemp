using System.Text.Json;

namespace CAMCMSServer.Model.ElementTypes;

public enum ElementType
{
    Text = 1,
    Image = 2,
    Audio = 3,
    Video = 4,
    Pdf = 5,
    Question = 6,
    Anchor = 7
}

public interface IElementType
{
    #region Properties

    ElementType ElementType { get; }

    bool Link { get; }

    #endregion

    #region Methods

    JsonDocument ConvertToJsonForPersistence();

    JsonDocument ConvertToJsonForUI();

    void ConvertFromJsonForPersistence(JsonDocument json);

    #endregion
}