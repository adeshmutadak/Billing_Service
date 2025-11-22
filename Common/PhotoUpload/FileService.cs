using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.PhotoUpload
{
    public class FileService : IFileService
    {
        public async Task<string?> SaveBase64ImageAsync(string base64Image, string folderPath)
        {
            if (string.IsNullOrEmpty(base64Image))
                return null;

            try
            {
                var parts = base64Image.Split(',');
                var cleanBase64 = parts.Length > 1 ? parts[1] : parts[0]; 

                byte[] imageBytes = Convert.FromBase64String(cleanBase64);

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string fileName = Guid.NewGuid().ToString() + ".png";
                string fullPath = Path.Combine(folderPath, fileName);

                await File.WriteAllBytesAsync(fullPath, imageBytes);

                return fullPath; // Absolute path stored in DB
            }
            catch
            {
                return null;
            }
        }
    }

}
