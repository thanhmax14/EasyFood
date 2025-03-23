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
                    Img = c.Img,
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

            // Kiểm tra nếu số thứ tự (Number) đã tồn tại ở danh mục khác
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

            // Nếu có ảnh mới, lưu vào thư mục và cập nhật đường dẫn
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ImageFile.FileName)}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Lưu ảnh mới
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(stream);
                }

                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrEmpty(category.Img))
                {
                    string oldImagePath = Path.Combine(uploadsFolder, category.Img);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                category.Img = uniqueFileName; // Lưu tên file vào DB
            }

            // Cập nhật thông tin danh mục
            category.Name = model.Name;
            category.Commission = model.Commission;
            category.Number = model.Number;
            category.ModifiedDate = DateTime.UtcNow;

            _context.SaveChanges();
        }

        public Categories GetCategoryById(Guid id)
        {
            return _context.Categories.FirstOrDefault(c => c.ID == id);
        }
    }
}
