using System.Web.Http;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Model.Requests;
using CAMCMSServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CAMCMSServer.Controller;

[Microsoft.AspNetCore.Mvc.Route("[controller]")]
[ApiController]
public class ModuleController : ControllerBase
{
    #region Data members

    private readonly ILibraryFolderService libraryFolderService;
    private readonly IModuleService moduleService;
    private readonly IElementSetService setRepository;

    private readonly IUserService userService;

    #endregion

    #region Constructors

    public ModuleController(IModuleService moduleService, IElementSetService setRepository,
        IUserService userService, ILibraryFolderService libraryFolderService)
    {
        this.moduleService = moduleService;
        this.setRepository = setRepository;
        this.userService = userService;
        this.libraryFolderService = libraryFolderService;
    }

    #endregion

    #region Methods

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("/[controller]-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> GetModules([FromUri] int userId, [FromUri] string userAccessToken)
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

        var folders = await this.libraryFolderService.GetAuthorizedLibraryFolders(userId);

        if (folders == null)
        {
            return NotFound();
        }

        foreach (var libraryFolder in folders)
        {
            var folderModules = await this.moduleService.GetAllInLibraryFolder(libraryFolder);
            libraryFolder.Modules = folderModules.ToList();
        }

        return Ok(folders);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("LoadById-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> GetModulesById([Microsoft.AspNetCore.Mvc.FromBody] int id,
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

        var module = await this.moduleService.GetByModuleId(id);

        if (module == null)
        {
            return NotFound();
        }

        var readable = await this.userService.ValidateLibraryRequest(userId, (int)module.LibraryFolderId!, "read");
        if (!readable)
        {
            return Unauthorized();
        }

        return Ok(module);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("LibraryLoad-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> GetModulesAsync([Microsoft.AspNetCore.Mvc.FromBody] LibraryFolder folder,
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

        var modules = await this.moduleService.GetAllInLibraryFolder(folder);
        return Ok(modules);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Load-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> GetModule([Microsoft.AspNetCore.Mvc.FromBody] Module module,
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

        var readable = await this.userService.ValidateLibraryRequest(userId, (int)module.LibraryFolderId!, "read");
        if (!readable)
        {
            return Unauthorized();
        }

        module = await this.moduleService.GetByModuleId(module.ModuleId);

        module.ElementSets = await this.setRepository.GetByModule(module);

        foreach (var moduleElementSet in module.ElementSets)
        {
            if (moduleElementSet != null)
            {
                moduleElementSet.Styling ??= new SetStyling
                {
                    is_appendix = false,
                    is_horizontal = false,
                    has_page_break = "false"
                };
            }
        }

        return Ok(module);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Create-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> CreateModule([Microsoft.AspNetCore.Mvc.FromBody] Module module,
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

        var creatable = await this.userService.ValidateLibraryRequest(userId, (int)module.LibraryFolderId!, "create");
        if (!creatable)
        {
            return Unauthorized(userId);
        }

        module.CreatedBy = userId;

        var newModule = await this.moduleService.Create(module);
        return Ok(newModule);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Update-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> UpdateModule([Microsoft.AspNetCore.Mvc.FromBody] Module module,
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

        var updateable = await this.userService.ValidateLibraryRequest(userId, (int)module.LibraryFolderId!, "update");
        if (!updateable)
        {
            return Unauthorized(userId);
        }

        await this.moduleService.Update(module);
        return Ok();
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("UpdateFolderId-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> UpdateModuleLibraryIdAsync([Microsoft.AspNetCore.Mvc.FromBody] Module module,
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

        var updateable = await this.userService.ValidateLibraryRequest(userId, (int)module.LibraryFolderId!, "update");
        if (!updateable)
        {
            return Unauthorized(userId);
        }

        await this.moduleService.UpdateLibraryFolder(module);
        return Ok();
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("UpdateFolderIdWithElements-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> UpdateModuleLibraryIdWithElementsAsync(
        [Microsoft.AspNetCore.Mvc.FromBody] Module module,
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

        var updateable = await this.userService.ValidateLibraryRequest(userId, (int)module.LibraryFolderId!, "update");
        if (!updateable)
        {
            return Unauthorized(userId);
        }

        await this.moduleService.UpdateLibraryFolderWithElements(module);
        return Ok();
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Delete-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> DeleteModule([Microsoft.AspNetCore.Mvc.FromBody] Module module,
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

        var deletable = await this.userService.ValidateLibraryRequest(userId, (int)module.LibraryFolderId!, "delete");
        if (!deletable)
        {
            return Unauthorized(userId);
        }

        await this.moduleService.Delete(module);
        return Ok(module);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("HasPublished-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> HasPublishedModule([Microsoft.AspNetCore.Mvc.FromBody] Module module,
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

        var deletable = await this.userService.ValidateLibraryRequest(userId, (int)module.LibraryFolderId!, "delete");
        if (!deletable)
        {
            return Unauthorized(userId);
        }

        var result = await this.moduleService.HasPublishedModule(module);

        return Ok(result);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("LocationAttributeUpdate-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> UpdateLocationAttribute(
        [Microsoft.AspNetCore.Mvc.FromBody] ElementLocation location,
        [FromUri] int userId, [FromUri] string userAccessToken) // TODO Move to own Controller if adding a second fetch for element location directly
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

        var libraryId = (await this.moduleService.GetModuleFromElementLocation(location)).LibraryFolderId;

        if (!await this.userService.ValidateLibraryRequest(userId, (int)libraryId!, "update"))
        {
            return Unauthorized();
        }

        await this.moduleService.UpdateLocationAttribute(location);
        return Ok(location);
    }

    #endregion
}