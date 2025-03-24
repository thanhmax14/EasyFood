using Microsoft.EntityFrameworkCore;
using Models;
using Models.DBContext;
using Repository.BaseRepository;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Categorys
{
    public class CategoryRepository : BaseRepository<Categories>, ICategoryRepository
    {
        public CategoryRepository(EasyFoodDbContext context) : base(context) {
            _context = context;
        }
        private readonly EasyFoodDbContext _context;
        public async Task<Categories> GetByNameAsync(string name)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);
        }
        public async Task<IEnumerable<Categories>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }
        public async Task<List<CategoryListViewModel>> GetCategoryListAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Number) // Sắp xếp theo thứ tự hiển thị
                .Select(c => new CategoryListViewModel
                {
                    ID = c.ID,
                    Img = !string.IsNullOrEmpty(c.Img) ? "/uploads/CategoryImage/" + c.Img : "/uploads/default.png",
                    Name = c.Name,
                    Number = c.Number,
                    Commission = c.Commission,
                    CreatedDate = c.CreatedDate,
                    ModifiedDate = c.ModifiedDate
                })
                .ToListAsync();
        }

        public void CreateCategory(CategoryCreateViewModel model)
        {
            // Kiểm tra nếu Number đã tồn tại trong cơ sở dữ liệu
            bool isNumberExists = _context.Categories.Any(c => c.Number == model.Number);
            if (isNumberExists)
            {
                throw new Exception("The display order (Number) already exists. Please choose another.");
            }

            string imagePath = null;
            if (model.Image != null && model.Image.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/CategoryImage");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = $"{Guid.NewGuid()}_{model.Image.FileName}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.Image.CopyTo(stream);
                }

                imagePath = $"/uploads/CategoryImage/{uniqueFileName}"; // Lưu đường dẫn tương đối
            }

            var category = new Categories
            {
                ID = model.ID,
                Name = model.Name,
                Commission = model.Commission,
                Number = model.Number,
                Img = imagePath, // Lưu đường dẫn ảnh vào DB
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = null
            };

            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public bool IsNumberExists(int number)
        {
            return _context.Categories.Any(c => c.Number == number);
        }

        public void UpdateCategory(CategoryUpdateViewModel model)
        {
            var category = _context.Categories.FirstOrDefault(c => c.ID == model.ID);
            if (category == null)
            {
                throw new Exception("Category not found.");
            }

            // Kiểm tra nếu số thứ tự (Number) đã tồn tại
            bool isNumberExists = _context.Categories.Any(c => c.Number == model.Number && c.ID != model.ID);
            if (isNumberExists)
            {
                throw new Exception("The display order (Number) already exists. Please choose another.");
            }

            // Đường dẫn thư mục lưu ảnh
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/CategoryImage");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Xử lý upload ảnh mới nếu có
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(model.ImageFile.FileName).ToLower();

                // Kiểm tra định dạng file
                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new Exception("Only JPG, JPEG, and PNG formats are allowed.");
                }

                // Giới hạn dung lượng file (5MB)
                if (model.ImageFile.Length > 5 * 1024 * 1024)
                {
                    throw new Exception("File size must be less than 5MB.");
                }

                // Tạo tên file duy nhất
                string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Lưu ảnh vào thư mục
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(stream);
                }

                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrEmpty(category.Img))
                {
                    string oldImagePath = Path.Combine(uploadsFolder, category.Img);
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }

                // Cập nhật ảnh mới vào DB
                category.Img = uniqueFileName;
            }

            // Cập nhật thông tin danh mục
            category.Name = model.Name;
            category.Commission = model.Commission;
            category.Number = model.Number;
            category.ModifiedDate = DateTime.UtcNow.Date; // Chỉ lấy ngày, không lấy giờ

            _context.SaveChanges();
        }

        public Categories GetCategoryById(Guid id)
        {
            return _context.Categories.FirstOrDefault(c => c.ID == id);
        }
    }
}
