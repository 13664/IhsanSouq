using System;
using System.Text;
using System.Text.Json;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using API.Services; 

namespace API.Controllers
{
[Route("api/[controller]")]
[ApiController]
    public class MulticardController : ControllerBase
    {
        private readonly IMulticardService _multicardService;

        public MulticardController(IMulticardService multicardService)
        {
            _multicardService = multicardService;
        }

        [HttpGet("auth-token")]
        public async Task<IActionResult> GetAuthToken()
        {
            try
            {
                var token = await _multicardService.GetAuthTokenAsync();
                return Ok(new { token });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
