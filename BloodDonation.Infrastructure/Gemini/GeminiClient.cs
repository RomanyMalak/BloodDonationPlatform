using BloodDonation.Infrastructure.Gemini;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace BloodDonation.Infrastructure.Services;

public class GeminiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;


    public GeminiClient(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;

        _apiKey = configuration["Gemini:ApiKey"]
            ?? throw new Exception("Gemini key missing");

        _model = configuration["Gemini:Model"]
            ?? "gemini-2.5-flash";
    }


    public async Task<string> AnalyzeAsync(
        byte[] image,
        string mimeType,
        CancellationToken cancellationToken)
    {

        var body = new
        {
            contents = new[]
            {
                new
                {
                    parts = new object[]
                    {
                        new
                        {
                            inline_data = new
                            {
                                mime_type = mimeType,
                                data = Convert.ToBase64String(image)
                            }
                        },

                        new
                        {
                            text = MedicalPrompt.Get()
                        }
                    }
                }
            }
        };


        var url =
        $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";


        var content = new StringContent(
            JsonSerializer.Serialize(body),
            Encoding.UTF8,
            "application/json");


        var response =
            await _httpClient.PostAsync(
                url,
                content,
                cancellationToken);


        var result =
            await response.Content
            .ReadAsStringAsync(cancellationToken);


        if (!response.IsSuccessStatusCode)
            throw new Exception(result);


        return result;
    }
}