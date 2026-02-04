using System.Web.Http;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CAMCMSServer.Controller
{
    [Microsoft.AspNetCore.Mvc.Route("[controller]")]
    [ApiController]
    public class OrganizationContentRoleController : ControllerBase
    {
        #region Data members

        private readonly IUserService userService;

        private readonly IOrganizationContentRoleService organizationContentRoleService;

        #endregion

        #region Constructors

        public OrganizationContentRoleController(IUserService userService, IOrganizationContentRoleService organizationContentRoleService)
        {
            this.userService = userService;
            this.organizationContentRoleService = organizationContentRoleService;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Creates a OrganizationContentRole for the organization
        /// </summary>
        /// <precondition>organizationContentRole must be > 0</precondition>
        /// <precondition>userId must be > 0</precondition>
        /// <param name="organizationContentRole">The organization content role.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userAccessToken">The user access token.</param>
        /// <returns>OkObjectRequest if the organization was created correctly otherwise returns BadObjectRequest</returns>
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("Create-{userId:int}&{userAccessToken}")]
        public async Task<IActionResult> CreateOrganization(
            [Microsoft.AspNetCore.Mvc.FromBody] OrganizationContentRole organizationContentRole, [FromUri] int userId, [FromUri] string userAccessToken)
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
                organizationContentRole.CreatedBy = userId;

                await this.organizationContentRoleService.CreateOrganizationContentRole(organizationContentRole);

                return Ok(organizationContentRole);
            }
            catch (Exception)
            {
                return BadRequest("Failed to create " + organizationContentRole.DisplayName);
            }
        }
        /// <summary>
        /// Gets the OrganizationContentRoles for the given organizationId 
        /// </summary>
        /// <precondition>organizationId must be > 0</precondition>
        /// <precondition>userId must be > 0</precondition>
        /// <param name="organizationId">The organization identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>OkObjectRequest with the OrganizationContentRoles if successful otherwise returns BadObjectRequest </returns>
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("GetContentRoles-{userId:int}&{userAccessToken}")]
        public async Task<IActionResult> GetContentRoles([Microsoft.AspNetCore.Mvc.FromBody] int organizationId,
            [FromUri] int userId, [FromUri] string userAccessToken)
        {
            if (userId <= 0)
            {
                return BadRequest(userId);
            }

            if (organizationId <= 0)
            {
                return BadRequest(organizationId);
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
                var contentRoles =
                    await this.organizationContentRoleService
                        .GetOrganizationContentRolesForOrganization(organizationId);

                return Ok(contentRoles);
            }
            catch (Exception)
            {
                return BadRequest("Failed to get content roles for organization " + organizationId);
            }
        }
        /// <summary>
        /// Deletes the organization content role using its id.
        /// </summary>
        /// <precondition>organizationContentRoleId must be > 0</precondition>
        /// <precondition>userId must be > 0</precondition>
        /// <param name="organizationContentRoleId">The organization content role identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userAccessToken">The user's access token.</param>
        /// <returns>OkObjectResult if successfully deletes the OrganizationContentRole otherwise returns BadObjectRequest</returns>
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("Delete-{userId:int}&{userAccessToken}")]
        public async Task<IActionResult> DeleteOrganizationContentRole(
            [Microsoft.AspNetCore.Mvc.FromBody] int organizationContentRoleId,
            [FromUri] int userId, [FromUri] string userAccessToken)
        {
            if (userId <= 0)
            {
                return BadRequest(userId);
            }

            if (organizationContentRoleId <= 0)
            {
                return BadRequest(organizationContentRoleId);
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
                await this.organizationContentRoleService.DeleteOrganizationContentRole(organizationContentRoleId);

                return Ok(organizationContentRoleId);
            }
            catch (Exception)
            {
                return BadRequest("Failed to delete organization content role " + organizationContentRoleId);
            }
        }

        #endregion
    }
}
