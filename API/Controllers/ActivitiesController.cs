using Application.Activities;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{    
    public class ActivitiesController : BaseApiController
    {
        //From first implementation is going to be removed  injecting our data insde this controled
        //and instead we will use mediator
  
        //Create two endpoints
        [HttpGet] //api/activities
        public async Task<IActionResult> GetActivities([FromQuery] ActivityParams param)
        {
            //return HandleResult(await Mediator.Send(new List.Query()));   
            return HandlePagedResult(await Mediator.Send(new List.Query { Params = param })); //pagination new method in BaseApiController
        }       

        [HttpGet("{id}")] //api/activities/fdfjhdjh used Id  
        public async Task<IActionResult> GetActivity(Guid id)
        {
            //In order to use mediator we need to register the mediator as service inside our program class
            return HandleResult(await Mediator.Send(new Details.Query{Id = id}));
        }

        [HttpPost]
        //we dont need any result then we get access to IActionResult like ok, bad request, not found 
        public async Task<IActionResult> CreateActivity(Activity activity)
        {
            return HandleResult(await Mediator.Send(new Create.Commnad{Activity = activity}));
        }
        
        [Authorize(Policy = "IsActivityHost")]
        [HttpPut("{id}")] //Used for updating resources
        //we dont need any result then we get access to IActionResult like ok, bad request, not found 
        public async Task<IActionResult> EditActivity(Guid id, Activity activity)
        {
            activity.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command{Activity = activity}));
        }

        [Authorize(Policy = "IsActivityHost")]
        [HttpDelete("{id}")] //Used for updating resources
        //we dont need any result then we get access to IActionResult like ok, bad request, not found 
        public async Task<IActionResult> DeleteActivity(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command{Id = id}));
        }

        [HttpPost("{id}/attend")]
        public async Task<IActionResult> Attend(Guid id)
        {
            return HandleResult(await Mediator.Send(new UpdateAttendance.Command{Id = id}));
        }
    }
}