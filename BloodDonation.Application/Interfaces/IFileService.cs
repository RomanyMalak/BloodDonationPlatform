using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Infrastructure.Services
{
    public interface IFileService
    {
        // ميثود بتاخد الملف الحقيقي واسم الفولدر، وترجع الـ Path اللي هيتسيف في الداتا بيز
        Task<string> UploadFileAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default);
    }
}
