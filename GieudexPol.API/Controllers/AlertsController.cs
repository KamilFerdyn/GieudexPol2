using GieudexPol.Application.Interfaces;
using GieudexPol.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GieudexPol.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertsController : ControllerBase
    {
        private readonly IUserAlertService _userAlertService;

        public AlertsController(IUserAlertService userAlertService)
        {
            _userAlertService = userAlertService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserAlerts(int userId)
        {
            var userAlerts = await _userAlertService.GetUserAlertsByUserIdAsync(userId);
            if (userAlerts == null)
            {
                return NotFound();
            }
            return Ok(userAlerts);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAlert([FromBody] UserAlert userAlert)
        {
            await _userAlertService.AddAsync(userAlert);
            return CreatedAtAction(nameof(GetUserAlerts), new { userId = userAlert.UserId }, userAlert);
        }
    }
}