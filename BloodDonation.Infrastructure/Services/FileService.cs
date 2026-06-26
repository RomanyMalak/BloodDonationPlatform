using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Infrastructure.Services
{
  
    public class FileService : IFileService
    {
       
        public async Task<string> UploadFileAsync(
         IFormFile file, 
         string folderName, 
         CancellationToken cancellationToken = default) 
        {

            if (file == null || file.Length == 0)
                throw new InvalidOperationException("No file was uploaded."); 

            // Get file extension in lower-case for validation.
            var extension = Path.GetExtension(file.FileName).ToLower();
            // Define allowed file extensions.
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };

            // Ensure the uploaded file's extension is allowed.
            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException("Invalid file type."); // Reject unsupported types.

            // Build the full uploads folder path under the application's current directory.
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folderName);

            // Ensure the uploads directory exists (creates it if missing).
            Directory.CreateDirectory(uploadsFolder);

            // Generate a new GUID-based filename to avoid collisions and append original extension.
            var fileName = $"{Guid.NewGuid()}{extension}";
            // Combine folder path and filename into a full file system path.
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Open a file stream for writing the uploaded file content.
            await using var stream = new FileStream(filePath, FileMode.Create);

            // Copy the uploaded file content into the destination stream asynchronously.
            await file.CopyToAsync(stream, cancellationToken);

            // Return the relative path that can be used to serve the file (from wwwroot).
            return $"uploads/{folderName}/{fileName}";
        }
    }
}
