using System;
using System.ComponentModel.DataAnnotations;

namespace Repository.ViewModels
{
    public class StoreViewModel
    {
        public Guid ID { get; set; }
        public string Name { get; set; } = default!;
        public string? LongDescriptions { get; set; }
        public string? ShortDescriptions { get; set; }
        public string? Address { get; set; }
        [Required(ErrorMessage = "Phone number cannot be blank!")]
        [RegularExpression(@"^(0\d{9}|\+84\d{9})$",
            ErrorMessage = "Phone number must be in the format: 0xxxxxxxxx or +84xxxxxxxxx")]
        public string? Phone { get; set; }
        public string? Img { get; set; }  // Hình ảnh cửa hàng
        public string? Status { get; set; } = "PENDING"; // Trạng thái mặc định
        public bool IsActive { get; set; } = false; // Mặc định chưa hoạt động
        public DateTime CreatedDate { get; set; } = DateTime.Now; // Ngày tạo mặc định
        public DateTime? ModifiedDate { get; set; }
        public string? UserID { get; set; } // ID của chủ cửa hàng (nếu cần)
        public string? UserName { get; set; }
        public int MyProperty { get; set; }
    }
}
