using BusinessLayer.ExternalServices.Abstractions;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
namespace BusinessLayer.ExternalServices.Implementations;

public class TelegramService : ITelegramService
{
    private readonly HttpClient _httpClient;
    private readonly string _botToken;
    private readonly string _chatId;

    public TelegramService(HttpClient httpClient, IConfiguration configuration) 
    {
        _httpClient = httpClient;
        _botToken = configuration["Telegram:BotToken"] ?? throw new Exception("Telegram BotToken not found.");
        _chatId = configuration["Telegram:ChatId"] ?? throw new Exception("Telegram ChatId not found.");
    }

    public async Task SendAsync(string message)
    {
        var url = $"https://api.telegram.org/bot{_botToken}/sendMessage";

        var payload = new
        {
            chat_id = _chatId,
            text = message,
            parse_mode = "HTML"
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Telegram mesajı göndərilə bilmədi: {error}");
        }
    }
}
