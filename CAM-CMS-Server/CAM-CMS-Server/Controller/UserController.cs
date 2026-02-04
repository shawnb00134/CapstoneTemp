using System.Web.Http;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Model.Requests;
using CAMCMSServer.Utils;
using CAMCMSServer.Utils.Notification;
using Microsoft.AspNetCore.Mvc;

namespace CAMCMSServer.Controller;

[Microsoft.AspNetCore.Mvc.Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    #region Data members

    private readonly ILibraryFolderService libraryFolderService;
    private readonly IUserService userService;
    private readonly INotificationService notificationService;

    #endregion

    #region Constructors

    public UserController(IUserService userService,
        ILibraryFolderService libraryFolderService, INotificationService notificationService)
    {
        this.userService = userService;
        this.libraryFolderService = libraryFolderService;
        this.notificationService = notificationService;
    }

    #endregion

    #region Methods

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("/[controller]-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> GetAllUsers([FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (string.IsNullOrEmpty(userAccessToken))
        {
            return BadRequest(userAccessToken);
        }

        if (userId <= 0)
        {
            return BadRequest(userId);
        }

        var userName = (await this.userService.GetUserById(userId)).Username;

        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName!) || !await this.isAdmin(userId))
        {
            return Unauthorized();
        }

        var users = await this.userService.GetUsers();
        return Ok(users);
    }

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("/TempUsers-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> GetAllTempUsers([FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (string.IsNullOrEmpty(userAccessToken))
        {
            return BadRequest(userAccessToken);
        }

        if (userId <= 0)
        {
            return BadRequest(userId);
        }

        var userName = (await this.userService.GetUserById(userId)).Username;

        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName!) || !await this.isAdmin(userId))
        {
            return Unauthorized();
        }

        var tempUsers = await this.userService.GetAllTempUsers();
        return Ok(tempUsers);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("GetUser-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> GetUser([Microsoft.AspNetCore.Mvc.FromBody] int id, [FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (string.IsNullOrEmpty(userAccessToken))
        {
            return BadRequest(userAccessToken);
        }

        if (userId <= 0)
        {
            return BadRequest(userId);
        }

        if (id <= 0)
        {
            return BadRequest(id);
        }
        
        var userName = (await this.userService.GetUserById(userId)).Username;

        if (!await this.isAdmin(userId) || !await TokenValidator.ValidateAccessToken(userAccessToken, userName!))
        {
            return Unauthorized();
        }

        var user = await this.userService.GetUserById(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("AllContexts-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> GetAllContexts([FromUri] int userId, [FromUri] string userAccessToken)
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

        if (!await this.isAdmin(userId) || !await TokenValidator.ValidateAccessToken(userAccessToken, userName!))
        {
            return Unauthorized();
        }

        var contexts = await this.userService.GetAllContexts();
        return Ok(contexts);
    }

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("AllRoles-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> GetAllRoles([FromUri] int userId, [FromUri] string userAccessToken)
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

        if (!await this.isAdmin(userId) || !await TokenValidator.ValidateAccessToken(userAccessToken, userName!))
        {
            return Unauthorized();
        }

        var roles = await this.userService.GetAllRoles();
        return Ok(roles);
    }

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("AllPrivileges-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> GetAllPrivileges([FromUri] int userId, [FromUri] string userAccessToken)
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

        if (!await this.isAdmin(userId) || !await TokenValidator.ValidateAccessToken(userAccessToken, userName!))
        {
            return Unauthorized();
        }

        var privileges = await this.userService.GetAllPrivileges();
        return Ok(privileges);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("UpdateUserPrivileges-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> UpdateUsersRole(
        [Microsoft.AspNetCore.Mvc.FromBody] UserAccessRoleRequest userRoleChange,
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

        if (!await this.isAdmin(userId) || !await TokenValidator.ValidateAccessToken(userAccessToken, userName!))
        {
            return Unauthorized();
        }
        var newUser = await this.userService.UpdateUserRoles(userRoleChange);
        return Ok(newUser);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("CreateUserPrivileges-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> CreateUsersRole(
        [Microsoft.AspNetCore.Mvc.FromBody] UserAccessRoleRequest userRoleChange,
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

        if (!await this.isAdmin(userId) || !await TokenValidator.ValidateAccessToken(userAccessToken, userName!))
        {
            return Unauthorized();
        }

        var newUser = await this.userService.CreateUserRoles(userRoleChange, userId);
        return Ok(newUser);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("DeleteUserPrivileges-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> DeleteUsersRole(
        [Microsoft.AspNetCore.Mvc.FromBody] UserAccessRoleRequest userRoleChange,
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

        if (!await this.isAdmin(userId) || !await TokenValidator.ValidateAccessToken(userAccessToken, userName!))
        {
            return Unauthorized();
        }

        var newUser = await this.userService.DeleteUserRoles(userRoleChange);
        return Ok(newUser);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("demoNotification-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> DemoNotification([FromUri] int userId)
    {
        if (userId <= 0)
        {
            return BadRequest(userId);
        }
        var user = await this.userService.GetUserById(userId);
        var subject = "Demo Notification";
        var body = "This is a demo notification from CAMCMS. If you received this, it means the notification system is working.";

        var success = await this.notificationService.SendNotification(user, subject, body);

        return success ? Ok() : StatusCode(500);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("SignUp-{invitationId:int}&{userAccessToken}")]
    public async Task<IActionResult> SignUp([Microsoft.AspNetCore.Mvc.FromBody] UserSignUpRequest request, [FromUri] int invitationId)
    {
        var tempUser = await userService.GetUserById(invitationId);
        if (tempUser == null )
        {
            return NotFound("Invitation not found or expired.");
        }

        tempUser.Username = request.Username;
        tempUser.Password = request.Password; 
        tempUser.Email = request.Email;
        tempUser.Firstname = request.Firstname;
        tempUser.Lastname = request.Lastname;

        await userService.UpdateUser(tempUser);

        return Ok();
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("AddUser-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> AddUser([Microsoft.AspNetCore.Mvc.FromBody] UserAddRequest request, [FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (userId <= 0)
        {
            return BadRequest("Invalid user ID.");
        }
    
        if (string.IsNullOrEmpty(userAccessToken))
        {
            return BadRequest("Access token is required.");
        }
    
        if (!await isAdmin(userId))
        {
            return Unauthorized("User is not authorized as an admin.");
        }
    
        var userName = (await this.userService.GetUserById(userId)).Username;
        if (string.IsNullOrEmpty(userName))
        {
            return NotFound("User not found.");
        }
    
        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName))
        {
            return Unauthorized("Invalid access token.");
        }
    
        var newUser = new TempUser
        {
            Username = request.Email,
            Firstname = request.Firstname,
            Lastname = request.Lastname,
            Email = request.Email,
            Phone = request.Phone,
            InvitationId = 1,
            AppUserId = 1
        };
    
         //var registerResponse = await userService.RegisterUserWithCognito(newUser);
        // if (registerResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
        // {
        //     return StatusCode((int)registerResponse.HttpStatusCode, "Failed to register user with Cognito.");
        // }
    
        //newUser.CognitoId = registerResponse.User.Username;

        await this.userService.CreateTempUser(newUser);
    
        return Ok(new { message = "User added successfully." });
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("DeleteUser-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> DeleteUser([Microsoft.AspNetCore.Mvc.FromBody] int deleteUserId, [FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (userId <= 0)
        {
            return BadRequest("Invalid user ID.");
        }

        if (string.IsNullOrEmpty(userAccessToken))
        {
            return BadRequest("Access token is required.");
        }

        if (!await isAdmin(userId))
        {
            return Unauthorized("User is not authorized as an admin.");
        }

        var userName = (await this.userService.GetUserById(userId)).Username;
        if (string.IsNullOrEmpty(userName))
        {
            return NotFound("User not found.");
        }

        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName))
        {
            return Unauthorized("Invalid access token.");
        }

        var result = await this.userService.DeleteUser(deleteUserId);
        if (!result)
        {
            return StatusCode(500, "Failed to delete user.");
        }

        return Ok(new { message = "User deleted successfully." });
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("DeleteTempUser-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> DeleteTempUser([Microsoft.AspNetCore.Mvc.FromBody] int deleteTempUserId, [FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (userId <= 0)
        {
            return BadRequest("Invalid user ID.");
        }

        if (string.IsNullOrEmpty(userAccessToken))
        {
            return BadRequest("Access token is required.");
        }

        if (!await isAdmin(userId))
        {
            return Unauthorized("User is not authorized as an admin.");
        }

        var userName = (await this.userService.GetUserById(userId)).Username;
        if (string.IsNullOrEmpty(userName))
        {
            return NotFound("User not found.");
        }

        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName))
        {
            return Unauthorized("Invalid access token.");
        }

        var result = await this.userService.DeleteTempUser(deleteTempUserId);
        if (!result)
        {
            return StatusCode(500, "Failed to delete temporary user.");
        }

        return Ok(new { message = "Temporary user deleted successfully." });
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("CheckAndUpdateUsers-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> CheckAndUpdateUsers()
    {
        var users = await userService.GetUsers();
        var tempUsers = await userService.GetAllTempUsers();

        foreach (var tempUser in tempUsers)
        {
            var matchingUser = users.FirstOrDefault(u => u.Username == tempUser.Email);

            if (matchingUser != null)
            {
                matchingUser.Email = tempUser.Email;
                matchingUser.Firstname = tempUser.Firstname;
                matchingUser.Lastname = tempUser.Lastname;
                matchingUser.Phone = tempUser.Phone;

                await userService.UpdateUser(matchingUser);

                tempUser.LinkedAppUserId = matchingUser.Id;
                tempUser.IsDeleted = true;

                await userService.UpdateTempUser(tempUser);
            }
        }

        return Ok(new { message = "Users checked and updated successfully." });
    }

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("IsAdmin-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> IsAdmin([FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (string.IsNullOrEmpty(userAccessToken))
        {
            return BadRequest(userAccessToken);
        }

        if (userId <= 0)
        {
            return BadRequest(userId);
        }

        var userName = (await this.userService.GetUserById(userId)).Username;

        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName!))
        {
            return Unauthorized();
        }

        var isAdmin = await this.userService.UserIsAdmin(userId);

        return Ok(new { isAdmin });
    }

    private async Task<bool> isAdmin(int userId)
    {
        return await this.userService.UserIsAdmin(userId);
    }

    #endregion
}