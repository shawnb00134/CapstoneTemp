using System.Web.Http;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CAMCMSServer.Controller;

[Microsoft.AspNetCore.Mvc.Route("[controller]")]
[ApiController]
public class PackageController : ControllerBase
{
    #region Data members

    private readonly IPackageService packageService;

    private readonly IUserService userService;

    #endregion

    #region Constructors

    public PackageController(IPackageService packageService, IUserService userService)
    {
        this.packageService = packageService;
        this.userService = userService;
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

        var userName = (await this.userService.GetUserById(userId)).Username!;
        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName))
        {
            return Unauthorized();
        }

        IEnumerable<Package>? packages = null;

        if (await this.userService.UserIsAdmin(userId))
        {
            packages = await this.packageService.GetAll();
        }
        else
        {
            packages = await this.packageService.GetAuthorizedPackages(userId);

            if (packages.IsNullOrEmpty())
            {
                return Unauthorized();
            }
        }

        return Ok(packages);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Load-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> GetPackageById([Microsoft.AspNetCore.Mvc.FromBody] int packageId,
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

        var userName = (await this.userService.GetUserById(userId)).Username!;
        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName))
        {
            return Unauthorized();
        }

        if (!await this.userService.ValidatePackageRequest(userId, packageId, "read"))
        {
            return Unauthorized();
        }

        var package = await this.packageService.GetById(packageId, userId);

        if (package == null)
        {
            return NotFound();
        }

        return Ok(package);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Create-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> CreatePackage([Microsoft.AspNetCore.Mvc.FromBody] Package package,
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

        var userName = (await this.userService.GetUserById(userId)).Username!;

        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName) ||
            !await this.userService.UserIsAdmin(userId))
        {
            return Unauthorized();
        }

        try
        {
            package.CreatedBy = userId;
            var newPackage = await this.packageService.Create(package);

            return newPackage == null
                ? throw new ArgumentException("New package was not created.")
                : Ok(newPackage);
        }
        catch (Exception)
        {
            return BadRequest("New package creation failed."); // TODO: Better error handling
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Update-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> UpdatePackage([Microsoft.AspNetCore.Mvc.FromBody] Package package,
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

        var userName = (await this.userService.GetUserById(userId)).Username!;

        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName))
        {
            return Unauthorized();
        }

        try
        {
            var newPackage = await this.packageService.Update(package);

            return newPackage == null
                ? throw new ArgumentException("Package was not updated.")
                : Ok(newPackage);
        }
        catch (Exception)
        {
            return BadRequest("Package update failed."); // TODO: Better error handling
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("AddFolder-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> AddPackageFolder([Microsoft.AspNetCore.Mvc.FromBody] PackageFolder folder,
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

        var userName = (await this.userService.GetUserById(userId)).Username!;

        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName))
        {
            return Unauthorized();
        }

        try
        {
            if (!await this.userService.ValidatePackageRequest(userId, (int)folder.PackageId, "create"))
            {
                return Unauthorized();
            }

            folder.CreatedBy = userId;
            var newPackage = await this.packageService.CreateFolder(folder);

            return newPackage == null
                ? throw new ArgumentException("New folder was not created.")
                : Ok(newPackage);
        }
        catch (Exception)
        {
            return BadRequest("New folder creation failed."); // TODO: Better error handling
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Delete-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> DeletePackage([Microsoft.AspNetCore.Mvc.FromBody] Package package,
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

        var userName = (await this.userService.GetUserById(userId)).Username!;

        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName))
        {
            return Unauthorized();
        }

        try
        {
            if (!await this.userService.UserIsAdmin(userId))
            {
                return Unauthorized();
            }

            await this.packageService.Delete(package);

            return Ok();
        }
        catch (Exception)
        {
            return BadRequest("Package deletion failed."); // TODO: Better error handling
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("MoveFolder-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> MovePackageFolder(
        [Microsoft.AspNetCore.Mvc.FromBody] PackageFolder packageFolder, [FromUri] int userId,
        [FromUri] string userAccessToken)
    {
        if (userId <= 0)
        {
            return BadRequest(userId);
        }

        if (string.IsNullOrEmpty(userAccessToken))
        {
            return BadRequest(userAccessToken);
        }

        var userName = (await this.userService.GetUserById(userId)).Username!;

        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName))
        {
            return Unauthorized();
        }

        if (packageFolder.PackageId == null)
        {
            return BadRequest(packageFolder.PackageId);
        }

        try
        {
            if (!await this.userService.ValidatePackageRequest(userId, (int)packageFolder.PackageId, "update"))
            {
                return Unauthorized();
            }
            await this.packageService.UpdateFolder(packageFolder);
            var packageData = await this.packageService.GetById((int)packageFolder.PackageId, userId);
            return Ok(packageData);
        }
        catch
        {
            return BadRequest("Package folder move failed."); // TODO: Better error handling
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("ReorderFolder-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> ReorderPackageFolder(
        [Microsoft.AspNetCore.Mvc.FromBody] PackageFolder packageFolder, [FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (userId <= 0)
        {
            return BadRequest(userId);
        }

        if (string.IsNullOrEmpty(userAccessToken))
        {
            return BadRequest(userAccessToken);
        }

        var userName = (await this.userService.GetUserById(userId)).Username!;

        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName))
        {
            return Unauthorized();
        }

        if (packageFolder.PackageId == null)
        {
            return BadRequest(packageFolder.PackageId);
        }

        try
        {
            if (!await this.userService.ValidatePackageRequest(userId, (int)packageFolder.PackageId, "update"))
            {
                return Unauthorized();
            }
            await this.packageService.ReorderFolder(packageFolder);

            var packageData = await this.packageService.GetById((int)packageFolder.PackageId, userId);
            return Ok(packageData);
        }
        catch
        {
            return BadRequest("Package folder reorder failed."); // TODO: Better error handling
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("DeleteFolder-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> DeletePackageFolder(
        [Microsoft.AspNetCore.Mvc.FromBody] PackageFolder packageFolder,
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

        var userName = (await this.userService.GetUserById(userId)).Username!;

        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName))
        {
            return Unauthorized();
        }

        if (packageFolder.PackageId == null)
        {
            return BadRequest(packageFolder.PackageId);
        }

        try
        {
            if (!await this.userService.ValidatePackageRequest(userId, (int)packageFolder.PackageId, "update"))
            {
                return Unauthorized();
            }
            await this.packageService.DeleteFolder(packageFolder);

            var package = await this.packageService.GetById((int)packageFolder.PackageId, userId);

            return Ok(package);
        }
        catch (Exception)
        {
            return BadRequest("Package deletion failed."); // TODO: Better error handling
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("UpdateFolder-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> UpdatePackageFolder(
        [Microsoft.AspNetCore.Mvc.FromBody] PackageFolder packageFolder,
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

        var userName = (await this.userService.GetUserById(userId)).Username!;

        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName))
        {
            return Unauthorized();
        }

        if (packageFolder.PackageId == null)
        {
            return BadRequest(packageFolder.PackageId);
        }

        try
        {
            await this.packageService.UpdateFolderContent(packageFolder);
            return Ok(packageFolder);
        }
        catch (Exception)
        {
            return BadRequest("Package folder update failed."); // TODO: Better error handling
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("GetSubFolders-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> GetSubFolders([Microsoft.AspNetCore.Mvc.FromBody] PackageFolder packageFolder,
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

        var userName = (await this.userService.GetUserById(userId)).Username!;
        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName))
        {
            return Unauthorized();
        }

        try
        {
            if (!await this.userService.ValidatePackageRequest(userId, await this.packageService.GetPackageIdFromFolder(packageFolder), "read"))
            {
                return Unauthorized();
            }

            var folders = (await this.packageService.GetSubFolders(packageFolder));

            return Ok(folders);
        }
        catch (Exception)
        {
            return BadRequest("Failed fetching packageFolders");
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("GetFolderModules-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> GetFolderModules([Microsoft.AspNetCore.Mvc.FromBody] PackageFolder packageFolder,
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

        var userName = (await this.userService.GetUserById(userId)).Username!;
        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName))
        {
            return Unauthorized();
        }

        try
        {
            var packageId = await this.packageService.GetPackageIdFromFolder(packageFolder);

            if (!await this.userService.ValidatePackageRequest(userId, packageId, "read"))
            {
                return Unauthorized();
            }

            var folders = (await this.packageService.GetAllSubFolderModules(packageFolder)).ToList();

            return Ok(folders);
        }
        catch (Exception)
        {
            return BadRequest("Failed fetching packageFolders");
        }
    }

    #endregion
}