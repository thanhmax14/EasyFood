using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusinessLogic.Services.ImageUpload
{
    public class ImageUploadService
    {
        private readonly HttpClient _httpClient;
        private const string IMGUR_UPLOAD_URL = "https://api.imgur.com/3/image";
        private const string IMGUR_CLIENT_ID = "YOUR_CLIENT_ID"; // Thay YOUR_CLIENT_ID bằng Client ID của bạn

        public ImageUploadService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Client-ID {IMGUR_CLIENT_ID}");
        }

        public async Task<string> UploadImageAsync(byte[] imageData)
        {
            var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(imageData), "image");

            var response = await _httpClient.PostAsync(IMGUR_UPLOAD_URL, content);
            var responseString = await response.Content.ReadAsStringAsync();

            using var jsonDoc = JsonDocument.Parse(responseString);
            var root = jsonDoc.RootElement;
            if (root.TryGetProperty("data", out var data) && data.TryGetProperty("link", out var link))
            {
                return link.GetString();
            }

            return null;
        }
    }
}
