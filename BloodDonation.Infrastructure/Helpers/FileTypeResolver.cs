namespace BloodDonation.Infrastructure.Helpers;

public static class FileTypeResolver
{
    public static string GetMimeType(string file)
    {
        return Path.GetExtension(file)
            .ToLower() switch
        {
            ".pdf" => "application/pdf",
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            _ => "image/jpeg"
        };
    }
}