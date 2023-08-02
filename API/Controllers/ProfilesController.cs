using Application.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProfilesController : BaseApiController
    {
        [HttpGet("{username}")]
        public async Task<IActionResult> GetProfile(string username)
        {
            //From Application\Profiles\Details.cs class
            return HandleResult(await Mediator.Send(new Details.Query { Username = username }));
        }

        [HttpPut]
        public async Task<IActionResult> Edit(EditProfile.Command command) // From Application\Profiles\EditProfile.cs
        {
            return HandleResult(await Mediator.Send(command));
        }
        //End point that clinet can send get request o “/api/profiles/{username}/activities?predicate
        [HttpGet("{username}/activities")]
        public async Task<IActionResult>GetActivitiesPerUser(string username, string predicate)
        {
            //From Application\Profiles\ListActivities.cs class
            return HandleResult(await Mediator.Send(new ListActivities.Query { Username = username, Predicate = predicate }));
        }
    }
}