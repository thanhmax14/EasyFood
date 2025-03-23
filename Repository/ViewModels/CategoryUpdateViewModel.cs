using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class CategoryUpdateViewModel
    {
        public Guid ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(0, 100, ErrorMessage = "Commission must be between 0 and 100.")]
        public float Commission { get; set; }

        [Required]
        public int Number { get; set; }

        public IFormFile? ImageFile { get; set; } // Ảnh upload mới
        public string? Img { get; set; } // Đường dẫn ảnh hiện tại

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
