using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.PhotoUpload
{
    public interface IFileService
    {
        Task<string?> SaveBase64ImageAsync(string base64Image, string folderPath);
    }
}
