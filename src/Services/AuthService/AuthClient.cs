using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hermes.Common;
using Hermes.DbCore;
using Hermes.Services.EmailService;
using Hermes.src.Extensions;
using Hermes.src.Models;
using Microsoft.IdentityModel.Tokens;
using RestSharp;

namespace Hermes.src.Services;
public class AuthClient(IMongoDbService<UserDetails> userDbService,IMongoDbService<OtpVerification> otpVerificationService,IEmailHelper emailHelper, ILogger<AuthClient> logger, IConfiguration configuration) : IAuthClient {
    private readonly IMongoDbService<UserDetails> _userDbService= userDbService;
    private readonly IMongoDbService<OtpVerification> _otpVerificationService= otpVerificationService;
    private readonly IEmailHelper _emailHelper= emailHelper;
    private readonly ILogger<AuthClient> _logger= logger;
    private readonly IConfiguration _configuration= configuration;
    public async Task Register(RegistrationInteraction registrationInteraction) {
        try {
            
            var user= await _userDbService.GetItemsAsync(x => x.Email == registrationInteraction.Email);
            if(user.Any()) {
                if(user.First().IsVerified) {
                    throw new HermesException(HermesException.BadRequest, "User already exists");
                }
                else{
                    await _userDbService.DeleteAsync(user.First().Id);
                }
            }
            var newUser = registrationInteraction.ToUserDetails();
            newUser.PublicUsername = await GeneratePublicUsername(registrationInteraction);
            await _userDbService.SaveAsync(newUser);
            _logger.LogInformation("User registered successfully");
            var otp = new OtpVerification {
                Email = registrationInteraction.Email,
                verificationCode = await GenerateOtp(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsValid = true
            };
            await _otpVerificationService.SaveAsync(otp);
            _logger.LogInformation("OTP generated successfully");
            var email = new EmailModel {
                ToEmails = new List<string> { registrationInteraction.Email },
                Subject = "Hermes - Verify your Account",
                Body = $"Your OTP is {otp.verificationCode}. It will expire in 5 minutes."
            };
            await _emailHelper.SendEmailAsync(email);
            _logger.LogInformation("Email sent successfully");

        } catch (HermesException e) {
            _logger.LogError($"Error registering user : {e.Message}");
            throw;
        }
        catch (Exception e) {
            _logger.LogError(e, "Error registering user");
            throw new HermesException(HermesException.InternalServerError, "Error registering user");
        }
    }

    public async Task<string> Login(LoginInteraction loginInteraction) {
        try {
            var userDetails = (await _userDbService.GetItemsAsync(x => (x.Password == HashPassword(loginInteraction.Password)) && ((x.Email == loginInteraction.EmailOrUsername) || (x.PublicUsername== loginInteraction.EmailOrUsername)))).ToList();
            if(userDetails.Count == 0) {
                throw new HermesException(HermesException.NotFound, "User not found");
            }
            if(userDetails.Count > 1) {
                throw new HermesException(HermesException.BadRequest, "Multiple users found with the same email or username");
            }
            if(userDetails.First().IsVerified == false) {
                throw new HermesException(HermesException.BadRequest, "User not verified");
            }
            return generateJwtToken(userDetails.First());
        }
        catch(HermesException e) {
            _logger.LogError($"Error logging in user : {e.Message}");
            throw;
        }
        catch(Exception e){
            _logger.LogError(e, "Error logging in user");
            throw new HermesException(HermesException.InternalServerError, "Error logging in user");
        }
    }
    public async Task VerifyAndRegisterAsync(OtpVerificationInteraction otpVerificationInteraction) {
        try {
            var user = (await _userDbService.GetItemsAsync(x => x.Email == otpVerificationInteraction.Email)).FirstOrDefault();
            var otp= (await _otpVerificationService.GetItemsAsync( x=> x.Email == otpVerificationInteraction.Email)).FirstOrDefault(); 
            if(otp == null) {
                throw new HermesException(HermesException.NotFound, "OTP not found");
            }
            if (user == null) {
                throw new HermesException(HermesException.NotFound, "User not found");
            }
            if (!otp.IsValid) {
                throw new HermesException(HermesException.BadRequest, "Invalid OTP");
            }
            if(DateTime.UtcNow > otp.ExpiresAt) {
                throw new HermesException(HermesException.BadRequest, "OTP expired");
            }
            if(otpVerificationInteraction.Code != otp.verificationCode) {
                throw new HermesException(HermesException.BadRequest, "Incorrect OTP");
            }
            user.IsVerified = true;
            await _userDbService.SaveAsync(user);
        } catch (HermesException e) {
            _logger.LogError($"Error verifying and registering user : {e.Message}");
            throw;
        } catch (Exception e) {
            _logger.LogError(e, "Error verifying and registering user");
            throw new HermesException(HermesException.InternalServerError, "Error verifying and registering user");
        }
    }

    private string generateJwtToken(UserDetails user) {
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                [
                    new Claim(CustomClaimTypes.UserId, user.UserId),
                    new Claim(ClaimTypes.GivenName, user.FirstName),
                    new Claim(ClaimTypes.Surname, user.LastName),
                    new Claim(ClaimTypes.Role, Role.USER.ToString()),
                ]
            ),
            Expires = DateTime.UtcNow.AddHours(48),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<String> GeneratePublicUsername(RegistrationInteraction registrationInteraction) {
        //Todo : Change Logic
        var client = new RestClient("https://usernameapiv1.vercel.app");
        var request = new RestRequest("api/random-usernames", Method.Get);
        var response = await client.ExecuteAsync(request);
        var usernameResponse = JsonSerializer.Deserialize<UsernameResponse>(response.Content);
        System.Console.WriteLine(JsonSerializer.Serialize(usernameResponse));
        var count = new Random().Next(10, 100);

        while((await _userDbService.GetItemsAsync(x => x.PublicUsername == usernameResponse.Usernames.First())).Any()) { 
            usernameResponse.Usernames[0]= $"{usernameResponse.Usernames.First()}{count}";
            count++;
        }
        return usernameResponse.Usernames.First();
    }

    private static string HashPassword(string password) {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
    }
    private async Task<string> GenerateOtp()
    {
        var random = new Random();
        var otp = new StringBuilder();
        for (int i = 0; i < 6; i++)
        {
            otp.Append(random.Next(0, 10));
        }
        return otp.ToString();
    }
}
class UsernameResponse {
    
    [JsonPropertyName("usernames")]
    public List<string> Usernames { get; set; }
}
