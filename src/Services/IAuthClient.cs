using Hermes.src.Models;

namespace Hermes.src.Services;
public interface IAuthClient {
    public Task Register(RegistrationInteraction registrationInteraction);
    public Task<string> Login(LoginInteraction loginInteraction);
}