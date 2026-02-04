using System.Web.Http;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CAMCMSServer.Controller;

[Microsoft.AspNetCore.Mvc.Route("/Module")]
[ApiController]
public class ModuleEditsController : ControllerBase
{
    #region Data members

    private readonly IElementSetService setService;
    private readonly IUserService userService;

    #endregion

    #region Constructors

    public ModuleEditsController(IElementSetService setService, IUserService userService)
    {
        this.setService = setService;
        this.userService = userService;
    }

    #endregion

    #region Methods

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("AddSet-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> AddSet([Microsoft.AspNetCore.Mvc.FromBody] ElementSet newSet, [FromUri] int userId,
        [FromUri] string userAccessToken)
    {
        if (newSet == null)
        {
            return BadRequest("No new set given.");
        }

        if (newSet.ModuleId == null)
        {
            return BadRequest("Set must have a valid ModuleId.");
        }

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

        var libraryFolderId = await this.setService.GetLibraryFolderIdFromModuleId(newSet.ModuleId);
        var updateable = await this.userService.ValidateLibraryRequest(userId, libraryFolderId, "update");
        if (!updateable)
        {
            return Unauthorized();
        }

        try
        {
            await this.setService.Create(newSet); // TODO: Return boolean

            var module = new Module
            {
                ModuleId = (int)newSet.ModuleId
            };

            var updatedSets = await this.setService.GetByModule(module);
            return Ok(updatedSets);
        }
        catch (Exception)
        {
            return BadRequest("New set creation failed."); // TODO: Better error handling
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("DeleteSet-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> DeleteSet([Microsoft.AspNetCore.Mvc.FromBody] ElementSet deleteSet, [FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (deleteSet == null)
        {
            return BadRequest("No new set given.");
        }

        if (deleteSet.ModuleId == null)
        {
            return BadRequest("Set must have a valid ModuleId.");
        }

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

        var libraryFolderId = await this.setService.GetLibraryFolderIdFromModuleId(deleteSet.ModuleId);
        var updateable = await this.userService.ValidateLibraryRequest(userId, libraryFolderId, "update");
        if (!updateable)
        {
            return Unauthorized();
        }

        try
        {
            await this.setService.Delete(deleteSet); // TODO: Return boolean

            var module = new Module
            {
                ModuleId = (int)deleteSet.ModuleId
            };

            var updatedSets = await this.setService.GetByModule(module);
            return Ok(updatedSets);
        }
        catch (Exception)
        {
            return BadRequest("Set deletion failed."); // TODO: Better error handling
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("MoveSet-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> MoveSet([Microsoft.AspNetCore.Mvc.FromBody] ElementSet updatedSet, [FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (updatedSet == null)
        {
            return BadRequest("No updated set given.");
        }

        if (updatedSet.ModuleId == null)
        {
            return BadRequest("Set must have a valid ModuleId.");
        }

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

        var libraryFolderId = await this.setService.GetLibraryFolderIdFromModuleId(updatedSet.ModuleId);
        var updateable = await this.userService.ValidateLibraryRequest(userId, libraryFolderId, "update");
        if (!updateable)
        {
            return Unauthorized();
        }

        try
        {
            await this.setService.UpdateSetOrder(updatedSet); // TODO: Return boolean

            var module = new Module
            {
                ModuleId = (int)updatedSet.ModuleId
            };

            var updatedSets = await this.setService.GetByModule(module);
            return Ok(updatedSets);
        }
        catch (Exception)
        {
            return BadRequest("Set movement failed."); // TODO: Better error handling
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("AddElement-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> AddElement([Microsoft.AspNetCore.Mvc.FromBody] ElementLocation newElement, [FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (newElement == null)
        {
            return BadRequest("No new element given.");
        }

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

        var libraryFolderId = await this.setService.GetParentModuleLibraryFolderIdFromElementLocation(newElement);
        var updateable = await this.userService.ValidateLibraryRequest(userId, libraryFolderId, "update");
        if (!updateable)
        {
            return Unauthorized();
        }

        try
        {
            await this.setService.AddElement(newElement); // TODO: Return boolean

            var updatedSet = await this.setService.GetById(newElement.SetLocationId);
            return Ok(updatedSet);
        }
        catch (Exception)
        {
            return BadRequest("New element creation failed."); // TODO: Better error handling
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("DeleteElement-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> DeleteElement([Microsoft.AspNetCore.Mvc.FromBody] ElementLocation deleteElement, [FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (deleteElement == null)
        {
            return BadRequest("No new set given.");
        }

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

        var libraryFolderId = await this.setService.GetParentModuleLibraryFolderIdFromElementLocation(deleteElement);
        var updateable = await this.userService.ValidateLibraryRequest(userId, libraryFolderId, "update");
        if (!updateable)
        {
            return Unauthorized();
        }

        try
        {
            await this.setService.DeleteElement(deleteElement); // TODO: Return boolean

            var updatedSet = await this.setService.GetById(deleteElement.SetLocationId);
            return Ok(updatedSet);
        }
        catch (Exception)
        {
            return BadRequest("Element deletion failed."); // TODO: Better error handling
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("MoveElement-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> MoveElement([Microsoft.AspNetCore.Mvc.FromBody] ElementLocation updatedElement, [FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (updatedElement == null)
        {
            return BadRequest("No new set given.");
        }

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

        var libraryFolderId = await this.setService.GetParentModuleLibraryFolderIdFromElementLocation(updatedElement);
        var updateable = await this.userService.ValidateLibraryRequest(userId, libraryFolderId, "update");
        if (!updateable)
        {
            return Unauthorized();
        }

        try
        {
            await this.setService.UpdateLocationOrder(updatedElement); // TODO: Return boolean

            var updatedSet = await this.setService.GetById(updatedElement.SetLocationId);
            return Ok(updatedSet);
        }
        catch (Exception)
        {
            return BadRequest("Element movement failed."); // TODO: Better error handling
        }
    }

    // NOTE: The list of locations is in order of [current, new]
    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("MoveElementSet-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> MoveElementSet([Microsoft.AspNetCore.Mvc.FromBody] List<ElementLocation> locations, [FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (locations == null)
        {
            return BadRequest("No updated set given.");
        }

        switch (locations.Count)
        {
            case > 2:
                return BadRequest("Only give the old location and new location.");
            case < 2:
                return BadRequest("Please provide the old location and new location.");
        }

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

        var libraryFolderId = await this.setService.GetParentModuleLibraryFolderIdFromElementLocation(locations[0]);
        var updateable = await this.userService.ValidateLibraryRequest(userId, libraryFolderId, "update");
        if (!updateable)
        {
            return Unauthorized();
        }

        try
        {
            await this.setService.UpdateLocationSet(locations[0], locations[1].SetLocationId); // TODO: Return boolean

            var set = await this.setService.GetById(locations[1].SetLocationId);
            var module = new Module
            {
                ModuleId = set.ModuleId ?? 0
            };
            var sets = await this.setService.GetByModule(module);

            return Ok(sets);
        }
        catch (Exception)
        {
            return BadRequest("Element movement failed."); // TODO: Better error handling
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("MoveElementSetPlace-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> MoveElementSetPlace(
        [Microsoft.AspNetCore.Mvc.FromBody] List<ElementLocation> locations, [FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (locations == null)
        {
            return BadRequest("No updated set given.");
        }

        switch (locations.Count)
        {
            case > 2:
                return BadRequest("Only give the old location and new location.");
            case < 2:
                return BadRequest("Please provide the old location and new location.");
        }

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

        var libraryFolderId = await this.setService.GetParentModuleLibraryFolderIdFromElementLocation(locations[0]);
        var updateable = await this.userService.ValidateLibraryRequest(userId, libraryFolderId, "update");
        if (!updateable)
        {
            return Unauthorized();
        }

        try
        {
            await this.setService.UpdateLocationSet(locations[0], locations[1].SetLocationId);

            locations[0].SetLocationId = locations[1].SetLocationId;
            locations[0].Place = locations[1].Place;
            await this.setService.UpdateLocationOrder(locations[0]);

            var set = await this.setService.GetById(locations[1].SetLocationId);
            var module = new Module
            {
                ModuleId = set.ModuleId ?? 0
            };
            var sets = await this.setService.GetByModule(module);

            return Ok(sets); // TODO: Add return of data to re-render
        }
        catch (Exception)
        {
            return BadRequest("Element movement failed."); // TODO: Better error handling
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("SwapElement-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> SwapElement([Microsoft.AspNetCore.Mvc.FromBody] ElementLocation updatedLocation, [FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (updatedLocation == null)
        {
            return BadRequest("No updated set given.");
        }

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

        var libraryFolderId = await this.setService.GetParentModuleLibraryFolderIdFromElementLocation(updatedLocation);
        var updateable = await this.userService.ValidateLibraryRequest(userId, libraryFolderId, "update");
        if (!updateable)
        {
            return Unauthorized();
        }

        try
        {
            await this.setService.UpdateLocationElement(updatedLocation); // TODO: Return boolean

            var updatedSet = await this.setService.GetById(updatedLocation.SetLocationId);
            return Ok(updatedSet);
        }
        catch (Exception)
        {
            return BadRequest("Set update failed."); // TODO: Better error handling
        }
    }

    /**
     * This is used when any attribute of a single set changes such as the editable property and the set attributes.
     */
    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("EditSet-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> UpdateSet([Microsoft.AspNetCore.Mvc.FromBody] ElementSet updatedSet, [FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (updatedSet == null)
        {
            return BadRequest("No updated set given.");
        }

        if (updatedSet.ModuleId == null)
        {
            return BadRequest("Set must have a valid ModuleId.");
        }

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

        var libraryFolderId = await this.setService.GetLibraryFolderIdFromModuleId(updatedSet.ModuleId);
        var updateable = await this.userService.ValidateLibraryRequest(userId, libraryFolderId, "update");
        if (!updateable)
        {
            return Unauthorized();
        }

        try
        {
            await this.setService.UpdateSet(updatedSet); // TODO: Return boolean

            var module = new Module
            {
                ModuleId = (int)updatedSet.ModuleId
            };

            var updatedSets = await this.setService.GetByModule(module);
            return Ok(updatedSets);
        }
        catch (Exception)
        {
            return BadRequest("Set update failed."); // TODO: Better error handling
        }
    }

    /**
     * This is used when any attribute of a single element changes such as the editable property and the location attributes.
     */
    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("EditElement-{userId:int}&{userAccessToken}")]
    public async Task<IActionResult> UpdateElement([Microsoft.AspNetCore.Mvc.FromBody] ElementLocation updatedElement, [FromUri] int userId, [FromUri] string userAccessToken)
    {
        if (updatedElement == null)
        {
            return BadRequest("No updated set given.");
        }

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

        var libraryFolderId = await this.setService.GetParentModuleLibraryFolderIdFromElementLocation(updatedElement);
        var updateable = await this.userService.ValidateLibraryRequest(userId, libraryFolderId, "update");
        if (!updateable)
        {
            return Unauthorized();
        }

        try
        {
            await this.setService.UpdateLocation(updatedElement); // TODO: Return boolean

            var updatedSet = await this.setService.GetById(updatedElement.SetLocationId);
            return Ok(updatedSet);
        }
        catch (Exception)
        {
            return BadRequest("Set update failed."); // TODO: Better error handling
        }
    }

    #endregion
}