using GieudexPol.Application.Auth.Commands;
using GieudexPol.Application.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GieudexPol.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            var command = new RegisterUserCommand { RegisterRequest = request };
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            var command = new LoginUserCommand { LoginRequest = request };
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}