using Hermes.src.Models;

namespace Hermes.Services.EmailService
{
    public interface IEmailHelper
    {
      Task SendEmailAsync(EmailModel email);
    }
}