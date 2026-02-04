using System.Web.Http;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CAMCMSServer.Controller;

[Microsoft.AspNetCore.Mvc.Route("[controller]")]
[ApiController]
public class LibraryFolderController : ControllerBase
{
    #region Data members

    private readonly ILibraryFolderService libraryFolderService;

    private readonly IModuleService moduleService;

    private readonly IElementService elementService;

    private readonly IUserService userService;

    #endregion

    #region Constructors

    public LibraryFolderController(ILibraryFolderService libraryFolderService,
        IUserService userService, IModuleService moduleService, IElementService elementService)
    {
        this.libraryFolderService = libraryFolderService;
        this.userService = userService;
        this.moduleService = moduleService;
        this.elementService = elementService;
    }

    #endregion

    #region Methods

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("/[controller]-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> GetFoldersAsync([FromUri] int userId, [FromUri] string userAccessToken)
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

        var allFolders = await this.libraryFolderService.GetAll();

        var folders = new List<LibraryFolder>();
        foreach (var folder in allFolders)
        {
            var readable = await this.userService.ValidateLibraryRequest(userId, folder.LibraryFolderId, "read");
            if (readable)
            {
                folders.Add(folder);
            }
        }

        return Ok(folders);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("LoadItems-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> GetFolderItems([FromUri] int userId,
        [Microsoft.AspNetCore.Mvc.FromBody] LibraryFolder libraryFolder, [FromUri] string userAccessToken)
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

        var readable = await this.userService.ValidateLibraryRequest(userId, libraryFolder.LibraryFolderId, "read");
        if (readable)
        {
            Task<IEnumerable<Module>> modulesTask = null;
            Task<IEnumerable<Element>> elementsTask = null;

            Parallel.Invoke(
                () => { modulesTask = this.moduleService.GetAllInLibraryFolder(libraryFolder); },
                () => { elementsTask = this.elementService.GetAllInLibraryFolder(libraryFolder); }
            );

            await Task.WhenAll(modulesTask, elementsTask);

            var modules = modulesTask.Result.ToList();
            var elements = elementsTask.Result.ToList();
            libraryFolder.Modules = modules;
            libraryFolder.Elements = elements;
        }

        return Ok(libraryFolder);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Create-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> CreateLibraryFolderAsync([FromUri] int userId,
        [Microsoft.AspNetCore.Mvc.FromBody] LibraryFolder folder, [FromUri] string userAccessToken)
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

        folder.CreatedBy = userId;
        await this.libraryFolderService.Create(folder);

        return Ok();
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Delete-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> DeleteLibraryFolderAsync([FromUri] int userId,
        [Microsoft.AspNetCore.Mvc.FromBody] LibraryFolder folder, [FromUri] string userAccessToken)
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

        try
        {
            folder.CreatedBy = userId;
            await this.libraryFolderService.Delete(folder);
            return Ok();
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    #endregion
}