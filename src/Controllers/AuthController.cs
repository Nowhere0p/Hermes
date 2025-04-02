using Hermes.Common;
using Hermes.src.Models;
using Hermes.src.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hermes.src.Controllers;

[ApiController]
[Route("api/v1.0")]
public class AuthController(IAuthClient authClient, ILogger<AuthController> logger) : ControllerBase {
    private readonly IAuthClient _authClient = authClient;
    private readonly ILogger<AuthController> _logger = logger;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationInteraction registrationInteraction) {
        try {
            await _authClient.Register(registrationInteraction);
            return Ok("User registered successfully");
        } catch (HermesException e) {
            _logger.LogError($"Error registering user : {e.Message}");
            return BadRequest(e.Message);
        } catch (Exception e) {
            _logger.LogError(e, "Error registering user");
            return StatusCode(500, "Error registering user");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginInteraction loginInteraction) {
        try {
            var token = await _authClient.Login(loginInteraction);
            return Ok(token);
        } catch (HermesException e) {
            _logger.LogError($"Error logging in user : {e.Message}");
            throw;
        }
        catch (Exception e) {
            _logger.LogError(e, "Error logging in user");
            throw new HermesException(HermesException.InternalServerError, "Error logging in user");
        }
    }
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "USER")]
    // [AllowAnonymous]
    [HttpGet("welcome")]
    public async Task<IActionResult> Welcome() {

        throw new HermesException(HermesException.BadRequest, "Unauthorized access");
        return Ok("Welcome to Hermes!");
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] OtpVerificationInteraction otpVerificationInteraction) {
        try {
            await _authClient.VerifyAndRegisterAsync(otpVerificationInteraction);
            return Ok("OTP verified successfully");
        } catch (HermesException e) {
            _logger.LogError($"Error verifying OTP : {e.Message}");
            return BadRequest(e.Message);
        } catch (Exception e) {
            _logger.LogError(e, "Error verifying OTP");
            return StatusCode(500, "Error verifying OTP");
        }
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