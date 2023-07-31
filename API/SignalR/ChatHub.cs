using Microsoft.AspNetCore.SignalR;
using Application.Comments;
using MediatR;

namespace API.SignalR
{
    public class ChatHub: Hub
    {
        private readonly IMediator _mediator;

        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }
        //Used in client in Comment Store in addComent
        public async Task SendComment(Create.Command command)
        {
            var comment = await _mediator.Send(command);
            //after the previous line when Create is executed comment has beeb saved in DB then its going to have comment ID
            // and is going to be shaped via auto mapper into a commentDTO and send that to abnybody who is connected to the hub, including the person that made the comment
            //Return the comment to themselves that can be displayed in the UI
            //We send to a group to match the activityID. Give a name of what to recieve what were seinding here
            //We need to use the name of this in the cliend side and then we can pass the comment that is a result object
            await Clients.Group(command.ActivityId.ToString()) // change from GUID to string
                .SendAsync("ReceiveComment", comment.Value);
        }

        //when a client connect to hub allow to joing to a group and can be used OnConnectedAsync inside hub when a client connects
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            //we just need to make sure that from Client we send this
            var activityId = httpContext.Request.Query["activityId"];
            //Object Groups from inside hub and it has that method to add a connected client to a group
            await Groups.AddToGroupAsync(Context.ConnectionId, activityId);
            //Send the list of comments and we have in Application the List Handler in Application\Comments\List.cs
            var result = await _mediator.Send(new List.Query{ActivityId = Guid.Parse(activityId)}); //from string to GUID
            await Clients.Caller.SendAsync("LoadComments", result.Value);
        }
    }            
}