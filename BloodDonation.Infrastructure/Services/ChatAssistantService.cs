using BloodDonation.Application.DTOs;
using BloodDonation.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BloodDonation.Infrastructure.Services;

/// <summary>
/// تنفيذ IChatAssistantService الذي يستدعي OpenPipe API.
/// منقول من مشروع BloodDonationAssistant المستقل، ومُدمج هنا داخل البنية الأساسية للمشروع.
/// </summary>
public class ChatAssistantService : IChatAssistantService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ChatAssistantService> _logger;
    private readonly string _apiKey;
    private readonly string _endpoint;
    private readonly string _model;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public ChatAssistantService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<ChatAssistantService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _apiKey = configuration["OpenPipe:ApiKey"]
            ?? throw new InvalidOperationException("OpenPipe:ApiKey غير موجود في appsettings.json");

        _endpoint = configuration["OpenPipe:Endpoint"]
            ?? throw new InvalidOperationException("OpenPipe:Endpoint غير موجود في appsettings.json");

        _model = configuration["OpenPipe:Model"]
            ?? throw new InvalidOperationException("OpenPipe:Model غير موجود في appsettings.json");
    }

    public async Task<ChatResponseDto> SendMessageAsync(string userMessage, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userMessage))
        {
            return new ChatResponseDto { Success = false, Error = "الرسالة لا يمكن أن تكون فاضية." };
        }

        var requestBody = new
        {
            model = _model,
            messages = new object[]
            {
                new { role = "system", content = "أنت مساعد صحي متخصص في التبرع بالدم. قدم إجابات مختصرة وآمنة وسهلة الفهم." },
                new { role = "user", content = userMessage }
            },
            store = true,
            temperature = 0
        };

        var json = JsonSerializer.Serialize(requestBody, JsonOptions);

        using var request = new HttpRequestMessage(HttpMethod.Post, _endpoint)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Authorization", $"Bearer {_apiKey}");

        try
        {
            var httpResponse = await _httpClient.SendAsync(request, cancellationToken);
            var responseBody = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger.LogError("OpenPipe API error {Status}: {Body}", httpResponse.StatusCode, responseBody);
                return new ChatResponseDto
                {
                    Success = false,
                    Error = $"خطأ {(int)httpResponse.StatusCode}: {httpResponse.ReasonPhrase}"
                };
            }

            using var doc = JsonDocument.Parse(responseBody);
            var answer = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;

            return new ChatResponseDto { Success = true, Answer = answer };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "فشل استدعاء OpenPipe API");
            return new ChatResponseDto { Success = false, Error = "تعذر الاتصال بخدمة المساعد الذكي. حاول مرة أخرى." };
        }
    }
}
