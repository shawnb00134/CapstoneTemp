using System.Web.Http;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CAMCMSServer.Controller;

[Microsoft.AspNetCore.Mvc.Route("[controller]")]
[ApiController]
public class PublishedModuleController : ControllerBase
{
    #region Data members

    private readonly IUserService userService;

    private readonly IPublishedModuleService publishedModuleService;

    #endregion

    #region Constructors

    public PublishedModuleController(IUserService userService, IPublishedModuleService publishedModuleService)
    {
        this.userService = userService;
        this.publishedModuleService = publishedModuleService;
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

        var username = (await this.userService.GetUserById(userId)).Username;
        if (!await TokenValidator.ValidateAccessToken(userAccessToken, username!))
        {
            return Unauthorized();
        }

        if (!await this.userService.UserIsAdmin(userId))
        {
            return Unauthorized();
        }

        var moduleRequest = await this.publishedModuleService.GetAll();

        var publishedModules = moduleRequest.ToList();

        return Ok(publishedModules);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("GetById-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> GetPublishedModuleById([Microsoft.AspNetCore.Mvc.FromBody] int publishedModuleId,
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

        var username = (await this.userService.GetUserById(userId)).Username;
        if (!await TokenValidator.ValidateAccessToken(userAccessToken, username!))
        {
            return Unauthorized();
        }

        var libraryFolderId = await this.publishedModuleService.GetLibraryFolderIdFromModuleId(publishedModuleId);
        var readable = await this.userService.ValidateLibraryRequest(userId, libraryFolderId, "read");
        if (!readable)
        {
            return Unauthorized();
        }


        var module = this.publishedModuleService.GetById(publishedModuleId).Result;
        return Ok(module);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Create-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> CreatePublishedModule([Microsoft.AspNetCore.Mvc.FromBody] PublishedModule module,
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

        var username = (await this.userService.GetUserById(userId)).Username;
        if (!await TokenValidator.ValidateAccessToken(userAccessToken, username!))
        {
            return Unauthorized();
        }

        var libraryFolderId = await this.publishedModuleService.GetLibraryFolderIdFromModuleId(module.Id);
        var creatable = await this.userService.ValidateLibraryRequest(userId, libraryFolderId, "create");
        if (!creatable)
        {
            return Unauthorized();
        }

        try
        {
            if (await this.publishedModuleService.GetById(module.Id) is { } checkingForModule)
            {
                await this.publishedModuleService.Update(module);
            }
            else
            {
                await this.publishedModuleService.Create(module);
            }
        }
        catch (Exception)
        {
            return BadRequest("Creation failed.");
        }

        return Ok();
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Update-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> UpdatePublishedModule([Microsoft.AspNetCore.Mvc.FromBody] PublishedModule module,
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

        var username = (await this.userService.GetUserById(userId)).Username;
        if (!await TokenValidator.ValidateAccessToken(userAccessToken, username!))
        {
            return Unauthorized();
        }

        var libraryFolderId = await this.publishedModuleService.GetLibraryFolderIdFromModuleId(module.Id);
        var updatable = await this.userService.ValidateLibraryRequest(userId, libraryFolderId, "update");
        if (!updatable)
        {
            return Unauthorized();
        }

        try
        {
            await this.publishedModuleService.Update(module);
        }
        catch (Exception)
        {
            return BadRequest("Update failed.");
        }
        return Ok();
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Delete-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> DeletePublishedModule([Microsoft.AspNetCore.Mvc.FromBody] PublishedModule module,
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

        var username = (await this.userService.GetUserById(userId)).Username;
        if (!await TokenValidator.ValidateAccessToken(userAccessToken, username!))
        {
            return Unauthorized();
        }

        var libraryFolderId = await this.publishedModuleService.GetLibraryFolderIdFromModuleId(module.Id);
        var deletable = await this.userService.ValidateLibraryRequest(userId, libraryFolderId, "delete");
        if (!deletable)
        {
            return Unauthorized();
        }

        try
        {
            await this.publishedModuleService.DeleteById(module);
        }
        catch (Exception)
        {
            return BadRequest("Deletion failed.");
        }
        return Ok();
    }

    #endregion
}