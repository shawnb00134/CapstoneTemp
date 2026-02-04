using System.Web.Http;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CAMCMSServer.Controller;

[Microsoft.AspNetCore.Mvc.Route("[controller]")]
[ApiController]
public class ElementSetController : ControllerBase
{
    #region Constructors

    public ElementSetController(IElementSetService elementSetService, IUserService authorizationService)
    {
        this.elementSetService = elementSetService;
        this.authorizationService = authorizationService;
    }

    #endregion

    #region Methods

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Update-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> UpdateElementSet([Microsoft.AspNetCore.Mvc.FromBody] ElementSet elementSet,
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

        var userName = (await this.authorizationService.GetUserById(userId)).Username;
        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName!))
        {
            return Unauthorized();
        }

        var libraryFolderId = await this.elementSetService.GetLibraryFolderIdFromElementSet(elementSet);
        var updateable = await this.authorizationService.ValidateLibraryRequest(userId, libraryFolderId, "update");
        if (!updateable)
        {
            return Unauthorized();
        }

        await this.elementSetService.Update(elementSet);
        return Ok(elementSet);
    }

    #endregion

    #region Data Members

    private readonly IElementSetService elementSetService;
    private readonly IUserService authorizationService;

    #endregion
}