using System.Web.Http;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CAMCMSServer.Controller;

[Microsoft.AspNetCore.Mvc.Route("[controller]")]
[ApiController]
public class PackageFolderModuleController : ControllerBase
{
    #region Data members

    private readonly IUserService userService;

    private readonly IPackageFolderModuleService packageFolderModuleService;

    #endregion

    #region Constructors

    public PackageFolderModuleController(IUserService userService,
        IPackageFolderModuleService packageFolderModuleService)
    {
        this.userService = userService;
        this.packageFolderModuleService = packageFolderModuleService;
    }

    #endregion

    #region Methods

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("/[controller]-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> GetAll([FromUri] int userId)
    {
        if (userId <= 0)
        {
            return BadRequest(userId);
        }

        var requestCheck = await this.userService.ValidateSystemRequest(userId, "read");
        if (!requestCheck)
        {
            return Unauthorized();
        }

        var result = await this.packageFolderModuleService.GetAll();
        return Ok(result);
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

        var requestCheck = await this.userService.ValidateSystemRequest(userId, "read");
        if (!requestCheck)
        {
            return Unauthorized();
        }

        return null;
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Create-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> CreatePackageFolderModule(
        [Microsoft.AspNetCore.Mvc.FromBody] PackageFolderModule module, [FromUri] int userId, [FromUri] string userAccessToken)
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
        
        var packageId = await this.packageFolderModuleService.GetPackageIdFromFolderId(module.PackageFolderId);
        var createable = await this.userService.ValidatePackageRequest(userId, packageId, "create");
        if (!createable)
        {
            return Unauthorized();
        }

        try
        {
            await this.packageFolderModuleService.Create(module);
            return Ok();
        }
        catch
        {
            return BadRequest();
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Update-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> UpdatePackageFolderModule(
        [Microsoft.AspNetCore.Mvc.FromBody] PackageFolderModule module,
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

        var packageId = await this.packageFolderModuleService.GetPackageIdFromFolderId(module.PackageFolderId);
        var updateable = await this.userService.ValidatePackageRequest(userId, packageId, "update");
        if (!updateable)
        {
            return Unauthorized();
        }

        try
        {
            await this.packageFolderModuleService.Update(module);

            return Ok();
        }
        catch
        {
            return BadRequest();
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("UpdateOrder-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> UpdatePackageModuleOrder(
        [Microsoft.AspNetCore.Mvc.FromBody] PackageFolderModule module, [FromUri] int userId, [FromUri] string userAccessToken)
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

        var packageId = await this.packageFolderModuleService.GetPackageIdFromFolderId(module.PackageFolderId);
        var updateable = await this.userService.ValidatePackageRequest(userId, packageId, "update");
        if (!updateable)
        {
            return Unauthorized();
        }


        try
        {
            await this.packageFolderModuleService.UpdateOrder(module);
            return Ok();
        }
        catch (Exception)
        {

            return BadRequest("Order Update failed.");
        }
       
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("DeleteById-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> DeletePublishedModule(
        [Microsoft.AspNetCore.Mvc.FromBody] PackageFolderModule module, [FromUri] int userId, [FromUri] string userAccessToken)
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

        var packageId = await this.packageFolderModuleService.GetPackageIdFromFolderId(module.PackageFolderId);
        var canDelete = await this.userService.ValidatePackageRequest(userId, packageId, "delete");
        if (!canDelete)
        {
            return Unauthorized();
        }

        try
        {
            await this.packageFolderModuleService.DeleteById(module);
            return Ok();
        }
        catch
        {
            return BadRequest("PackageFolderModule deletion failed");
        }
    }

    #endregion
}