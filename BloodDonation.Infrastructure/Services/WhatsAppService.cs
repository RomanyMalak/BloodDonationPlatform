using BloodDonation.Application.Interfaces;
using BloodDonation.Infrastructure.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace BloodDonation.Infrastructure.Services;

public class WhatsAppService : IWhatsAppService
{
    private readonly TwilioSettings _settings;
    private readonly ILogger<WhatsAppService> _logger;

    public WhatsAppService(
        IOptions<TwilioSettings> options,
        ILogger<WhatsAppService> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(
        string? to,
        string message,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(to))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_settings.AccountSid) ||
            string.IsNullOrWhiteSpace(_settings.AuthToken) ||
            string.IsNullOrWhiteSpace(_settings.FromWhatsApp))
        {
            _logger.LogWarning("WhatsApp message skipped because Twilio settings are incomplete.");
            return;
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        try
        {
            TwilioClient.Init(_settings.AccountSid, _settings.AuthToken);

            await MessageResource.CreateAsync(
                from: new PhoneNumber(_settings.FromWhatsApp),
                to: new PhoneNumber(FormatWhatsAppNumber(to)),
                body: message);
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(
                ex,
                "Twilio WhatsApp send failed for recipient {Recipient}.",
                to);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "WhatsApp notification failed for recipient {Recipient}.",
                to);
        }
    }

    private static string FormatWhatsAppNumber(string phoneNumber)
    {
        var formatted = phoneNumber.Trim();

        // Remove whatsapp: if it already exists
        if (formatted.StartsWith("whatsapp:", StringComparison.OrdinalIgnoreCase))
        {
            formatted = formatted["whatsapp:".Length..];
        }

        // Convert local Egyptian number to international format
        if (formatted.StartsWith("01"))
        {
            formatted = "+2" + formatted;
        }
        else if (formatted.StartsWith("20"))
        {
            formatted = "+" + formatted;
        }
        else if (!formatted.StartsWith("+"))
        {
            formatted = "+20" + formatted;
        }

        return $"whatsapp:{formatted}";
    }
}
