using System.Web.Http;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CAMCMSServer.Controller;

[Microsoft.AspNetCore.Mvc.Route("[controller]")]
[ApiController]
public class StudioAsideController : ControllerBase
{
    #region Data members

    #region Data Members

    private readonly IStudioAsideService studioAsideService;
    private readonly IUserService userService;

    #endregion

    #endregion

    #region Constructors

    #region Constructor

    public StudioAsideController(IStudioAsideService studioAsideService, IUserService userService)
    {
        this.studioAsideService = studioAsideService;
        this.userService = userService;
    }

    #endregion

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

        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName!))
        {
            return Unauthorized();
        }

        var asideContent = await this.studioAsideService.GetStudioAside(userId);

        return Ok(asideContent);
    }

    #endregion
}