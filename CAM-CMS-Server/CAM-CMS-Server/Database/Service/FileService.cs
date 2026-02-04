using System.Text.Json;
using CAMCMSServer.Model;
using CAMCMSServer.Model.ElementTypes;

namespace CAMCMSServer.Database.Service;

public interface IFileService
{
    #region Methods

    Task UploadFileAsync(IFormFile file, Element newElement);

    public IElementType CheckFileType(IFormFile file, Element element);

    Task DeleteFileAsync(string key);

    Task<string> GetFileAsync(string key);

    Task<string> GetFileUrlAsync(Element element, int baseElement);

    #endregion
}

public class DevFileService : LocalFileService
{
    #region Constructors

    public DevFileService()
    {
    }

    #endregion
}

public class ReviewFileService : LocalFileService
{
    #region Constructors

    public ReviewFileService()
    {
    }

    #endregion
}

public class LocalFileService : IFileService
{
    #region Constructors

    public LocalFileService()
    {
    }

    #endregion

    #region Methods

    public async Task UploadFileAsync(IFormFile file, Element newElement)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", newElement.ElementId + "-" + file.FileName);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
    }

    public IElementType CheckFileType(IFormFile file, Element element)
    {
        var extension = Path.GetExtension(file.FileName);
        switch (extension)
        {
            case ".pdf":
                {
                    var pdfElement = new PdfElement
                    {
                        Key = element.ElementId + "-" + file.FileName,
                        Link = false
                    };
                    return pdfElement;
                }
            case ".jpg":
            case ".jpeg":
            case ".png":
                return new ImageElement { Key = element.ElementId + "-" + file.FileName, Link = false };
            default:
                {
                    return null;
                }
        }
    }

    public Task DeleteFileAsync(string key)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", key);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }

    public Task<string> GetFileAsync(string key)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", key);
        return Task.FromResult(File.Exists(filePath) ? filePath : string.Empty);
    }

    public Task<string> GetFileUrlAsync(Element element, int baseElement)
    {
        return Task.FromResult(string.Empty);
    }

    #endregion



}