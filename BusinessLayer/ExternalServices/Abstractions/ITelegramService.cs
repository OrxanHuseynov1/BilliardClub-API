namespace BusinessLayer.ExternalServices.Abstractions;

public interface ITelegramService
{
    Task SendAsync(string message);
}
