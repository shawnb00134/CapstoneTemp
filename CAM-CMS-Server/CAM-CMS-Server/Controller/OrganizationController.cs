using System.Web.Http;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CAMCMSServer.Controller;

[Microsoft.AspNetCore.Mvc.Route("[controller]")]
[ApiController]
public class OrganizationController : ControllerBase
{
    #region Data members

    protected readonly IOrganizationService organizationService;

    private readonly IUserService userService;

    #endregion

    #region Constructors

    public OrganizationController(IOrganizationService organizationService, IUserService userService)
    {
        this.organizationService = organizationService;
        this.userService = userService;
    }

    #endregion

    #region Methods

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("/[controller]-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> GetAllAuthorizedOrganizations([FromUri] int userId, [FromUri] string userAccessToken)
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

        IEnumerable<Organization>? organizations = null;

        if (!(await this.isAdmin(userId)))
        {
            organizations = await this.organizationService.GetAuthorizedOrganizations(userId);

            if (organizations.IsNullOrEmpty())
            {
                return Unauthorized();
            }
        }
        else
        {
            organizations = await this.organizationService.GetAll();
        }

        return Ok(organizations);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Create-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> CreateOrganization(
        [Microsoft.AspNetCore.Mvc.FromBody] Organization organization, [FromUri] int userId, [FromUri] string userAccessToken)
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

        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName!) || ! await this.isAdmin(userId))
        {
            return Unauthorized();
        }
        
        try
        {
            await this.organizationService.Create(organization, userId);
            return Ok(organization);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("GetById-{userId:int}&{userAccessToken}")]
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

        try
        {
            var organization = await this.organizationService.GetById(id, userId);

            return Ok(organization);
        }
        catch
        {
            return BadRequest();
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Delete-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> DeleteOrganization([Microsoft.AspNetCore.Mvc.FromBody] int organizationId, [FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (userId <= 0)
        {
            return BadRequest("Invalid user ID.");
        }

        if (string.IsNullOrEmpty(userAccessToken))
        {
            return BadRequest("Access token is required.");
        }

        var userName = (await this.userService.GetUserById(userId)).Username;
        if (string.IsNullOrEmpty(userName))
        {
            return NotFound("User not found.");
        }

        if (!await this.isAdmin(userId))
        {
            return Unauthorized("User is not authorized as an admin.");
        }

        try
        {
            var result = await this.organizationService.DeleteOrganization(organizationId);
            if (!result)
            {
                return StatusCode(500, "Failed to delete organization.");
            }
            return Ok(new { message = "Organization deleted successfully." });
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Internal server error: {e.Message}");
        }
    }


    private async Task<bool> isAdmin(int userId)
    {
        return await this.userService.UserIsAdmin(userId);
    }

    #endregion
}