using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class ProductImageViewModel
    {
        public string ImageUrl { get; set; } // Đường dẫn hình ảnh
        public bool IsMain { get; set; } // Xác định ảnh chính

        // 🆕 Thuộc tính mới để xử lý ảnh tải lên
        public IFormFile ImageFile { get; set; } // Ảnh tải lên từ form
        public string FileName { get; set; } // Tên file ảnh
        public string ContentType { get; set; } // Loại file ảnh (JPEG, PNG)
        public byte[] ImageData { get; set; } // Dữ liệu ảnh dạng byte
    }
}
