using System.Web.Http;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CAMCMSServer.Controller;

[Microsoft.AspNetCore.Mvc.Route("[controller]")]
[ApiController]
public class OrganizationPackageController : ControllerBase
{
    #region Data members

    protected readonly IOrganizationPackageService organizationPackageService;
    private readonly IUserService userService;

    #endregion

    #region Constructors

    public OrganizationPackageController(IOrganizationPackageService organizationPackageService,
        IUserService userService)
    {
        this.organizationPackageService = organizationPackageService;
        this.userService = userService;
    }

    #endregion

    #region Methods

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("GetByPackageId-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> GetOrganizationById([Microsoft.AspNetCore.Mvc.FromBody] int id,
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

        var organization = await this.organizationPackageService.GetOrganizationIdsByPackageId(id, userId);

        return Ok(organization);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("UpdateAssociatedOrganizations-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> UpdateAssociatedOrganizations(
        [Microsoft.AspNetCore.Mvc.FromBody] PackageAndOrganizations packageAndOrganizations, [FromUri] int userId, [FromUri] string userAccessToken)
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

        await this.organizationPackageService.UpdateAssociatedOrganizations(packageAndOrganizations, userId);
        return Ok();
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("UpdateAssociatedPackages-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> UpdateAssociatedPackages(
        [Microsoft.AspNetCore.Mvc.FromBody] OrganizationAndPackages organizationAndPackages, [FromUri] int userId, [FromUri] string userAccessToken)
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

        try
        {
            await this.organizationPackageService.UpdateAssociatedPackages(organizationAndPackages, userId);
        }
        catch (Exception)
        {
            return BadRequest();
        }
        return Ok();
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("GetAllPackageByOrganizationId-{userId:int}&{userAccessToken}")]
    public async Task<IEnumerable<Package>> GetAllPackageByOrganizationId(
        [Microsoft.AspNetCore.Mvc.FromBody] int organizationId, [FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (userId <= 0)
        {
            return null;
        }

        if (string.IsNullOrEmpty(userAccessToken))
        {
            return null;
        }

        var userName = (await this.userService.GetUserById(userId)).Username;
        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName!))
        {
            return null;
        }

        var organizationPackages = await this.organizationPackageService.GetAllPackageByOrganizationId(organizationId, userId);

        return organizationPackages;
    }

    #endregion
}