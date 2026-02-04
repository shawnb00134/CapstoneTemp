using System.Text.Json;
using System.Web.Http;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Model.ElementTypes;
using CAMCMSServer.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CAMCMSServer.Controller;

[Microsoft.AspNetCore.Mvc.Route("[controller]")]
[ApiController]
public class ElementController : ControllerBase
{
    #region Data members

    private readonly ILibraryFolderService libraryFolderService;
    private readonly IElementService elementService;
    //private readonly IFileService fileService;

    private readonly IUserService userService;

    #endregion

    #region Constructors

    public ElementController(IElementService elementService, IUserService userService,
        ILibraryFolderService libraryFolderService)
    {
        this.elementService = elementService;
        this.userService = userService;
        this.libraryFolderService = libraryFolderService;
        //this.fileService = fileService;
    }

    #endregion

    #region Methods

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("/[controller]-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> GetAll([FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (userId <= 0)
        {
            return BadRequest(userId);
        }

        if (string.IsNullOrEmpty(userAccessToken))
        {
            return BadRequest(userAccessToken);
        }

        var userName = (await this.userService.GetUserById(userId)).Username;
        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName!) || !await this.userService.UserIsAdmin(userId))
        {
            return Unauthorized();
        }

        var allFolders = await this.libraryFolderService.GetAll();

        var folders = new List<LibraryFolder>();
        foreach (var folder in allFolders)
        {
            var folderRequest = await this.GetElementsAsync(folder, userId, userAccessToken);
            if (folderRequest is OkObjectResult result)
            {
                var elements = (List<Element>)result.Value!;
                folder.Elements = elements;
                folders.Add(folder);
            }
        }

        return Ok(folders);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("GetById-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> GetElementById([Microsoft.AspNetCore.Mvc.FromBody] int elementId,
        [FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (string.IsNullOrEmpty(userAccessToken))
        {
            return BadRequest(userAccessToken);
        }

        var userName = (await this.userService.GetUserById(userId)).Username;
        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName!))
        {
            return Unauthorized();
        }

        var element = this.elementService.GetById(elementId).Result;

        if (element == null)
        {
            return NotFound();
        }

        if (userId <= 0)
        {
            return BadRequest(userId);
        }

        var readable = await this.userService.ValidateLibraryRequest(userId, (int)element.LibraryFolderId!, "read");
        if (!readable)
        {
            return Unauthorized(userId);
        }

        var isLink = false;

        if (element.Content != null)
        {
            var elementContentJson = JObject.Parse(element.Content);

            if (elementContentJson.TryGetValue("Link", out var value))
            {
                isLink = value.Value<bool>();
            }
        }

        if (!isLink)
        {
            if (element.TypeId is (int)ElementType.Image or (int)ElementType.Pdf)
            {
                var elementImageJson = JsonSerializer.Deserialize<ImageElement>(element.Content);
                //element.Content = await this.fileService.GetFileAsync(elementImageJson.Key);
            }
        }

        if (element.TypeId == (int)ElementType.Anchor)
        {
            element.Content = string.Empty;
        }

        return Ok(element);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("LibraryLoad-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> GetElementsAsync([Microsoft.AspNetCore.Mvc.FromBody] LibraryFolder folder,
        [FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (userId <= 0)
        {
            return BadRequest(userId);
        }

        if (string.IsNullOrEmpty(userAccessToken))
        {
            return BadRequest(userAccessToken);
        }

        var userName = (await this.userService.GetUserById(userId)).Username;
        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName!))
        {
            return Unauthorized();
        }

        var readable = await this.userService.ValidateLibraryRequest(userId, folder.LibraryFolderId, "read");
        if (!readable)
        {
            return Unauthorized(userId);
        }

        var elements = await this.elementService.GetAllInLibraryFolder(folder);
        return Ok(elements);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Create-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> CreateElement([FromForm] Element element,
        [FromUri] int userId, [FromUri] string userAccessToken)
    {
        element.LibraryFolderId ??= 3;
        element.Tags ??= Array.Empty<string>();

        if (userId <= 0)
        {
            return BadRequest(userId);
        }

        if (string.IsNullOrEmpty(userAccessToken))
        {
            return BadRequest(userAccessToken);
        }

        var userName = (await this.userService.GetUserById(userId)).Username;
        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName!))
        {
            return Unauthorized();
        }

        var creatable = await this.userService.ValidateLibraryRequest(userId, (int)element.LibraryFolderId, "create");
        if (!creatable)
        {
            return Unauthorized(userId);
        }

        element.CreatedBy = userId;

        if (element.FormFile != null)
        {
            var newElement = await this.elementService.Create(element);

            //var elementType = this.fileService.CheckFileType(element.FormFile, newElement);

            //newElement.Content = JsonSerializer.Serialize(elementType.ConvertToJsonForPersistence());

            await this.elementService.Update(newElement);

            //await this.fileService.UploadFileAsync(element.FormFile, newElement);

            return Ok(newElement);
        }

        if (element.TypeId == 7)
        {
            var newElement = await this.elementService.Create(element);

            return Ok(newElement);
        }

        {
            if (element.Content == null || JsonSerializer.Deserialize<JsonDocument>(element.Content) == null)
            {
                return BadRequest(element);
            }

            var newElement = await this.elementService.Create(element);
            return Ok(newElement);
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Update-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> UpdateElement([Microsoft.AspNetCore.Mvc.FromBody] Element element,
        [FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (userId <= 0)
        {
            return BadRequest(userId);
        }

        if (string.IsNullOrEmpty(userAccessToken))
        {
            return BadRequest(userAccessToken);
        }

        var userName = (await this.userService.GetUserById(userId)).Username;
        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName!))
        {
            return Unauthorized();
        }

        var updateable = await this.userService.ValidateLibraryRequest(userId, (int)element.LibraryFolderId!, "update");
        if (!updateable)
        {
            return Unauthorized(userId);
        }

        element.UpdatedBy = userId;
        Element updatedElement = null;
        if (element.TypeId is (int)ElementType.Image or (int)ElementType.Pdf)
        {
            var elementBackend = await this.elementService.GetById((int)element.ElementId);
            var elementContent = JObject.Parse(element.Content);
            var backendContent = JObject.Parse(elementBackend.Content);
            //if (backendContent.ContainsKey("Key") && !elementContent.ContainsKey("Key"))
            //{
            //    await this.fileService.DeleteFileAsync(backendContent.GetValue("Key").Value<string>());
            //}
        }

        updatedElement = await this.elementService.Update(element);

        return Ok(updatedElement);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("UpdateWithFile-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> UpdateWithFile([FromForm] Element element,
        [FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (userId <= 0)
        {
            return BadRequest(userId);
        }

        if (string.IsNullOrEmpty(userAccessToken))
        {
            return BadRequest(userAccessToken);
        }

        var userName = (await this.userService.GetUserById(userId)).Username;
        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName!))
        {
            return Unauthorized();
        }

        var updateable = await this.userService.ValidateLibraryRequest(userId, (int)element.LibraryFolderId!, "update");
        if (!updateable)
        {
            return Unauthorized(userId);
        }

        var elementBackend = await this.elementService.GetById((int)element.ElementId);

        elementBackend.Description = element.Description;
        elementBackend.Title = element.Title;
        elementBackend.Tags = element.Tags;
        elementBackend.Citation = element.Citation;
        elementBackend.ExternalSource = element.ExternalSource;

        var elementBackendContent = JObject.Parse(elementBackend.Content);

        if (elementBackendContent.ContainsKey("Key"))
        {
            var key = elementBackendContent["Key"].ToString();
            //await this.fileService.DeleteFileAsync(key);

            //await this.fileService.UploadFileAsync(element.FormFile, elementBackend);

            elementBackendContent["Key"].Replace(elementBackend.ElementId + "-" + element.FormFile.FileName);
            elementBackend.Content = elementBackendContent.ToString(Formatting.None);
        }
        else
        {
            //await this.fileService.UploadFileAsync(element.FormFile, elementBackend);
            var fileJson = new JObject
            {
                ["ElementType"] = elementBackend.TypeId,
                ["Key"] = elementBackend.ElementId + "-" + element.FormFile.FileName
            };

            elementBackend.Content = fileJson.ToString(Formatting.None);
        }

        elementBackend = await this.elementService.Update(elementBackend);

        return Ok(elementBackend);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("UpdateLibraryId-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> UpdateLibraryId([Microsoft.AspNetCore.Mvc.FromBody] Element element,
        [FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (userId <= 0)
        {
            return BadRequest(userId);
        }

        if (string.IsNullOrEmpty(userAccessToken))
        {
            return BadRequest(userAccessToken);
        }

        var userName = (await this.userService.GetUserById(userId)).Username;
        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName!) || !await this.userService.UserIsAdmin(userId))
        {
            return Unauthorized();
        }

        var updatedElement = await this.elementService.UpdateLibraryFolder(element);
        return Ok(updatedElement);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Delete-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> DeleteElement([Microsoft.AspNetCore.Mvc.FromBody] Element element,
        [FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (userId <= 0)
        {
            return BadRequest(userId);
        }

        if (string.IsNullOrEmpty(userAccessToken))
        {
            return BadRequest(userAccessToken);
        }

        var userName = (await this.userService.GetUserById(userId)).Username;
        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName!))
        {
            return Unauthorized();
        }

        var deletable = await this.userService.ValidateLibraryRequest(userId, (int)element.LibraryFolderId!, "delete");
        if (!deletable)
        {
            return Unauthorized(userId);
        }

        var elementFileInfo = this.elementService.GetById((int)element.ElementId).Result;

        var deleted = await this.elementService.Delete(element);

        if (!deleted)
        {
            return Conflict(element);
        }

        if (element.TypeId is (int)ElementType.Image or (int)ElementType.Pdf)
        {
            var elementJson = JsonSerializer.Deserialize<ImageElement>(elementFileInfo.Content);
            //await this.fileService.DeleteFileAsync(elementJson.Key);
        }

        return Ok();
    }

    #endregion
}