using System.Web.Http;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CAMCMSServer.Controller;

[Microsoft.AspNetCore.Mvc.Route("[controller]")]
[ApiController]
public class ContentRoleController : ControllerBase
{
    #region Data members

    private readonly IContentRoleService contentRoleService;
    private readonly IUserService userService;

    #endregion

    #region Constructors

    #region Constructor

    public ContentRoleController(IContentRoleService contentRoleService, IUserService userService)
    {
        this.contentRoleService = contentRoleService;
        this.userService = userService;
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    ///     Gets all the content roles from the db and returns to the front end
    ///     if user is 0 or less will return a badrequest
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns></returns>
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

        var contentRoles = await this.contentRoleService.GetAllContentRoles();

        return Ok(contentRoles);
    }

    /// <summary>
    ///     Adds a new content role.
    /// </summary>
    /// <param name="contentRole">The content role to add.</param>
    /// <returns></returns>6
    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Create-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> AddContentRole([Microsoft.AspNetCore.Mvc.FromBody] ContentRoleRequest contentRoleRequest, [FromUri] int userId, [FromUri] string userAccessToken)
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

        try
        {
            var newContentRole = new ContentRole { Name = contentRoleRequest.Name, ArchetypeId = contentRoleRequest.ArchetypeId };
            var createdContentRole = await this.contentRoleService.AddContentRole(newContentRole);
            return Ok(createdContentRole);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("Archetypes-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> GetAllArchetypes([FromUri] int userId, [FromUri] string userAccessToken)
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

        try
        {
            var archetypes = await this.contentRoleService.GetAllArchetypes();
            return Ok(archetypes);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    public class ContentRoleRequest
    {
        public string Name { get; set; }
        public int ArchetypeId { get; set; }
    }

    #endregion
}