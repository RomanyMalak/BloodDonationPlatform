namespace BloodDonation.Application.Interfaces;

public interface IWhatsAppService
{
    Task SendAsync(
        string? to,
        string message,
        CancellationToken cancellationToken = default);
}
