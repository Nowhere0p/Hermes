using Hermes.Common;
using Hermes.Services.EmailService;
using Hermes.src.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace Hermes.src.Services;

public class EmailHelper(
    ISmtpClient smtpClient,
    ILogger<EmailHelper> logger,
    IConfigManager configs
) : IEmailHelper
{
    private readonly ISmtpClient _smtpClient = smtpClient;
    private readonly ILogger<EmailHelper> _logger = logger;
    private readonly IConfigManager _configs= configs;

    public async Task SendEmailAsync(EmailModel email)
    {
        try
        {
            var mail = new MimeMessage();
            var fromMailAddresses = (await _configs.GetConfigurationAsync()).FromMailAddresses;
            foreach(var sender in fromMailAddresses)
            {
                mail.From.Add(MailboxAddress.Parse(sender));
            }
            foreach (var receiver in email.ToEmails)
            {
                mail.To.Add(MailboxAddress.Parse(receiver));
            }
            mail.Subject = email.Subject;
            mail.Body = new TextPart("plain") { Text = email.Body };
            await _smtpClient.SendAsync(mail);
            _logger.LogInformation("EMAIL SENT");
        }
        catch (Exception ex)
        {
            throw new HermesException(
                HermesException.InternalServerError,
                $"Failed to Send Email: {ex.Message}"
            );
        }
    }
}
