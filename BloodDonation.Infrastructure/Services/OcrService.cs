using BloodDonation.Application.DTOs.Ocr;
using BloodDonation.Application.Interfaces;
using BloodDonation.Infrastructure.Helpers;
using BloodDonation.Infrastructure.Gemini;


namespace BloodDonation.Infrastructure.Services;


public class OcrService : IOcrService
{

    private readonly GeminiClient _gemini;
    private readonly GeminiResponseParser _parser;


    public OcrService(
    GeminiClient gemini,
    GeminiResponseParser parser)
    {
        _gemini = gemini;
        _parser = parser;
    }



    public async Task<OcrResultDto> VerifyAsync(
    string? documentUrl,
    string patientName,
    string? hospitalName,
    string bloodType,
    CancellationToken cancellationToken)
    {

        if (string.IsNullOrWhiteSpace(documentUrl))
            return new OcrResultDto
            {
                IsVerified = false,
                FailureReason = "No document provided"
            };



        var bytes =
        await LoadFile(documentUrl);



        var mime =
        FileTypeResolver.GetMimeType(documentUrl);



        var response =
        await _gemini.AnalyzeAsync(
        bytes,
        mime,
        cancellationToken);



        return _parser.Parse(
        response,
        bloodType);

    }




    private async Task<byte[]> LoadFile(string path)
    {

        if (path.StartsWith("http"))
        {
            using var client = new HttpClient();

            return await client.GetByteArrayAsync(path);
        }



        var fullPath =
        Path.Combine(
        Directory.GetCurrentDirectory(),
        "wwwroot",
        path.TrimStart('/'));



        return await File.ReadAllBytesAsync(fullPath);

    }

}