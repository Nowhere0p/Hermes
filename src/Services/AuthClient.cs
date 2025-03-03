using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hermes.Common;
using Hermes.DbCore;
using Hermes.src.Extensions;
using Hermes.src.Models;
using Microsoft.IdentityModel.Tokens;
using RestSharp;

namespace Hermes.src.Services;
public class AuthClient(IMongoDbService<UserDetails> mongoDbService, ILogger<AuthClient> logger, IConfiguration configuration) : IAuthClient {
    private readonly IMongoDbService<UserDetails> _mongoDbService= mongoDbService;
    private readonly ILogger<AuthClient> _logger= logger;
    private readonly IConfiguration _configuration= configuration;
    public async Task Register(RegistrationInteraction registrationInteraction) {
        try {
            if((await _mongoDbService.GetItemsAsync(x => x.Email == registrationInteraction.Email)).Any()) {
                throw new HermesException(HermesException.BadRequest, "User already exists");
            }
            var user = registrationInteraction.ToUserDetails();
            user.PublicUsername = await GeneratePublicUsername(registrationInteraction);
            await _mongoDbService.SaveAsync(user);
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
            // ToDO: Check isVerified
            var userDetails = (await _mongoDbService.GetItemsAsync(x => (x.Password == HashPassword(loginInteraction.Password)) && ((x.Email == loginInteraction.EmailOrUsername) || (x.PublicUsername== loginInteraction.EmailOrUsername)))).ToList();
            if(userDetails.Count == 0) {
                throw new HermesException(HermesException.NotFound, "User not found");
            }
            if(userDetails.Count > 1) {
                throw new HermesException(HermesException.BadRequest, "Multiple users found with the same email or username");
            }
            return generateJwtToken(userDetails.First());
        }
        catch(HermesException e) {
            _logger.LogError($"Error logging in user : {e.Message}");
            throw;
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

        while((await _mongoDbService.GetItemsAsync(x => x.PublicUsername == usernameResponse.Usernames.First())).Any()) { 
            usernameResponse.Usernames[0]= $"{usernameResponse.Usernames.First()}{count}";
            count++;
        }
        return usernameResponse.Usernames.First();
    }

    private static string HashPassword(string password) {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
    }
}
class UsernameResponse {
    
    [JsonPropertyName("usernames")]
    public List<string> Usernames { get; set; }
}