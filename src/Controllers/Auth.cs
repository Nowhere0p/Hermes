using Hermes.Common;
using Microsoft.AspNetCore.Mvc;

namespace Hermes.src.Controllers;

[ApiController]
[Route("api/v1.0")]
public class AuthController : ControllerBase {
    // [HttpPost("register")]
    // public async Task<IActionResult> Register([FromBody] UserDetails userDetails) {
    //     // Register user
    //     return Ok();
    // }

    // [HttpPost("login")]
    // public async Task<IActionResult> Login([FromBody] User user) {
    //     // Login user
    //     return Ok();
    // }

    [HttpGet("welcome")]
    public async Task<IActionResult> Welcome() {
        throw new HermesException(HermesException.BadRequest,"bad");
        // Logout user
        return Ok("Welcome to Hermes!");
    }

    // [HttpPost("forgot-password")]
    // public async Task<IActionResult> ForgotPassword([FromBody] User user) {
    //     // Forgot password
    //     return Ok();
    // }

    // [HttpPost("reset-password")]
    // public async Task<IActionResult> ResetPassword([FromBody] User user) {
    //     // Reset password
    //     return Ok();
    // }
}