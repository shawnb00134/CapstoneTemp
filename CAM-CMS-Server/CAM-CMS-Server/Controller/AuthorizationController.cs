using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Web.Http;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CAMCMSServer.Controller;

[Microsoft.AspNetCore.Mvc.Route("[controller]")]
[ApiController]
public class AuthorizationController : ControllerBase
{
    #region Constructors

    public AuthorizationController(IConfiguration configuration, IUserService userService,
        ILibraryFolderService libraryFolderService, IOrganizationService organizationService,
        IAuthenticationService authenticationService)
    {
        Configuration = configuration;
        //this.IdentityProvider = new AmazonCognitoIdentityProviderClient();
        //this.UserPool =
        //    new CognitoUserPool(this.Configuration.GetSection("AWS").GetValue<string>("UserPoolId"),
        //        this.Configuration.GetSection("AWS").GetValue<string>("UserPoolClientId"), this.IdentityProvider);

        this.userService = userService;
        this.libraryFolderService = libraryFolderService;
        this.organizationService = organizationService;
        this.authenticationService = authenticationService;
    }

    #endregion

    #region Data members

    protected readonly IConfiguration Configuration;
    //protected readonly IAmazonCognitoIdentityProvider IdentityProvider;
    //protected readonly CognitoUserPool UserPool;

    private readonly IUserService userService;

    private readonly ILibraryFolderService libraryFolderService;

    private readonly IOrganizationService organizationService;

    private readonly IAuthenticationService authenticationService;

    #endregion

    #region Methods

    [Microsoft.AspNetCore.Mvc.HttpPost]
    public async Task<IActionResult> Authorize([Microsoft.AspNetCore.Mvc.FromBody] User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(user.AccessToken);

        if (jwtToken.Payload.TryGetValue("username", out var username))
        {
            user.Username = username as string;

            var sessionUser = await findDbUser(user);
            if (sessionUser != null)
            {
                sessionUser.Timestamp = DateTime.Now.ToString(CultureInfo.InvariantCulture);

                return Ok(sessionUser);
            }
        }

        return Unauthorized();
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Login")]
    public async Task<IActionResult> Login([Microsoft.AspNetCore.Mvc.FromBody] User user)
    {
        var validUser = await this.authenticationService.AuthenticateUser(user.Username, user.Password);

        if (validUser != null)
        {
            user = validUser;
            var token = TokenValidator.GenerateUserAccessToken(user);
            user.AccessToken = token;
            user.RefreshToken = user.AccessToken;
            user.Password = "";

            var sessionUser = await findDbUser(user);
            if (sessionUser != null)
            {
                sessionUser.Timestamp = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                sessionUser.Password = "";

                return Ok(sessionUser);
            }
        }

        return Unauthorized();
    }

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("CheckToken-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> CheckToken([FromUri] string userAccessToken, [FromUri] int userId)
    {
        var userName = (await userService.GetUserById(userId)).Username;
        var valid = await TokenValidator.ValidateAccessToken(userAccessToken, userName!);

        return valid ? Accepted() : Unauthorized();
    }

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("System-{userId:int}&{userAccessToken}&{request}")]
    public async Task<IActionResult> AuthorizeSystem([FromUri] int userId, [FromUri] string request,
        [FromUri] string userAccessToken)
    {
        var authorized = await userService.ValidateSystemRequest(userId, request);

        return authorized ? Accepted() : Unauthorized();
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Org-{userId:int}&{userAccessToken}&{request}")]
    public async Task<IActionResult> AuthorizeOrg([FromUri] int userId, [FromUri] string request,
        [Microsoft.AspNetCore.Mvc.FromBody] int orgId, [FromUri] string userAccessToken)
    {
        try
        {
            var authorized = await userService.ValidateOrganizationRequest(userId, orgId, request);

            return authorized ? Accepted() : Unauthorized();
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("AnyOrg-{userId:int}&{userAccessToken}&{request}")]
    public async Task<IActionResult> AuthorizeAnyOrg([FromUri] int userId, [FromUri] string request,
        [FromUri] string userAccessToken)
    {
        var authTasks = new List<Task<bool>>();
        var organizations = await organizationService.GetAll();

        try
        {
            Parallel.ForEach(organizations,
                org =>
                {
                    authTasks.Add(userService.ValidateOrganizationRequest(userId, (int)org.OrganizationId!, request));
                });

            if (authTasks.IsNullOrEmpty())
            {
                var authorized = await userService.ValidateSystemRequest(userId, request);
                return authorized ? Accepted() : Unauthorized();
            }

            var authorization = await Task.WhenAll(authTasks);

            return authorization.Any(x => x) ? Accepted() : Unauthorized();
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Folder-{userId:int}&{userAccessToken}&{request}")]
    public async Task<IActionResult> AuthorizeFolder([FromUri] int userId, [FromUri] string request,
        [Microsoft.AspNetCore.Mvc.FromBody] int folderId, [FromUri] string userAccessToken)
    {
        if (userId <= 0 || string.IsNullOrEmpty(userAccessToken)) return Unauthorized();

        var userName = (await userService.GetUserById(userId)).Username;
        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName!)) return Unauthorized();

        var authorized = await userService.ValidateLibraryRequest(userId, folderId, request);

        return authorized ? Accepted() : Unauthorized();
    }

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("AnyFolder/-{userId:int}&{userAccessToken}&{request}")]
    public async Task<IActionResult> AuthorizeAnyFolder([FromUri] int userId, [FromUri] string request,
        [FromUri] string userAccessToken)
    {
        var authTasks = new List<Task<bool>>();
        var folders = await libraryFolderService.GetAll();

        Parallel.ForEach(folders,
            folder => { authTasks.Add(userService.ValidateLibraryRequest(userId, folder.LibraryFolderId, request)); });

        if (authTasks.IsNullOrEmpty())
        {
            var authorized = await userService.ValidateSystemRequest(userId, request);
            return authorized ? Accepted() : Unauthorized();
        }

        var authorization = await Task.WhenAll(authTasks);
        return authorization.Any(x => x) ? Accepted() : Unauthorized();
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("Package/-{userId:int}&{userAccessToken}&{request}")]
    public async Task<IActionResult> AuthorizePackage([FromUri] int userId, [FromUri] string userAccessToken,
        [FromUri] string request, [Microsoft.AspNetCore.Mvc.FromBody] int packageId)
    {
        if (userId <= 0 || string.IsNullOrEmpty(userAccessToken)) return Unauthorized();

        var userName = (await userService.GetUserById(userId)).Username;
        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName!)) return Unauthorized();

        var authorized = await userService.ValidatePackageRequest(userId, packageId, request);

        return authorized ? Accepted() : Unauthorized();
    }


    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("RefreshToken-{userRefreshToken}")]
    public async Task<IActionResult> RefreshToken([FromUri] string userRefreshToken)
    {
        //var authRequest = new InitiateAuthRequest
        //{
        //    AuthFlow = AuthFlowType.REFRESH_TOKEN,
        //    AuthParameters = new Dictionary<string, string> { { "REFRESH_TOKEN", userRefreshToken } },
        //    ClientId = this.Configuration.GetSection("AWS").GetValue<string>("UserPoolClientId")
        //};

        //var authResponse = await this.IdentityProvider.InitiateAuthAsync(authRequest);
        //if (authResponse.AuthenticationResult != null)
        //{
        //    return Ok(authResponse.AuthenticationResult);
        //

        return Unauthorized();
    }

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("Read/-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> CheckUserReadPrivilege([FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (userId <= 0) return BadRequest(userId);
        if (string.IsNullOrEmpty(userAccessToken)) return BadRequest(userAccessToken);
        var userName = (await userService.GetUserById(userId)).Username;
        if (!await TokenValidator.ValidateAccessToken(userAccessToken, userName!)) return Unauthorized();

        try
        {
            var userReadPrivileges = await userService.CheckReadPrivileges(userId);

            return Ok(userReadPrivileges);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    private async Task<User?> findDbUser(User user)
    {
        // Get username, password, first and last name, email and phone number from cognito
        //var response = this.UserPool.GetUser(user.CognitoId);
        user.Password = "";

        //foreach (var attributeType in response.Attributes)
        //{
        //    switch (attributeType.Key)
        //    {
        //        case "email":
        //            user.Email = attributeType.Value;
        //            break;
        //        //case "phone_number":
        //        //    user.Phone = attributeType.Value;
        //        //    break;
        //        case "given_name":
        //            user.Firstname = attributeType.Value;
        //            break;
        //        case "family_name":
        //            user.Lastname = attributeType.Value;
        //            break;
        //        default:
        //            continue;
        //    }
        //}

        var dbUser = await userService.ValidateUser(user);
        dbUser!.AccessToken = user.AccessToken;
        dbUser.RefreshToken = user.RefreshToken;
        return dbUser;
    }

    #endregion
}