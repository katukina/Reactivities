using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    
    public class BaseApiController : ControllerBase
    {
        private IMediator _mediator;
        //All the mediator classed that inherit from this can use it
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
    }
}