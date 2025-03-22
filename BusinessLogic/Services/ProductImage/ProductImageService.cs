using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Models;
using Repository.ProductImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.ProductImages
{
    public class ProductImageService : IProductImageService
    {
        private readonly IProductImageRepository _repository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public ProductImageService(IProductImageRepository repository, IMapper mapper, IWebHostEnvironment environment)
        {
            _repository = repository;
            _mapper = mapper;
            _environment = environment;
        }

        public IQueryable<ProductImage> GetAll() => _repository.GetAll();

        public ProductImage GetById(Guid id) => _repository.GetById(id);

        public async Task<ProductImage> GetAsyncById(Guid id) => await _repository.GetAsyncById(id);

        public ProductImage Find(Expression<Func<ProductImage, bool>> match) => _repository.Find(match);

        public async Task<ProductImage> FindAsync(Expression<Func<ProductImage, bool>> match) => await _repository.FindAsync(match);

        public async Task AddAsync(ProductImage entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(ProductImage entity) => await _repository.UpdateAsync(entity);

        public async Task DeleteAsync(ProductImage entity) => await _repository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _repository.ExistsAsync(id);
        public int Count() => _repository.Count();

        public async Task<int> CountAsync() => await _repository.CountAsync();

        public async Task<IEnumerable<ProductImage>> ListAsync() => await _repository.ListAsync();

        public async Task<IEnumerable<ProductImage>> ListAsync(
            Expression<Func<ProductImage, bool>> filter = null,
            Func<IQueryable<ProductImage>, IOrderedQueryable<ProductImage>> orderBy = null,
            Func<IQueryable<ProductImage>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ProductImage, object>> includeProperties = null) =>
            await _repository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _repository.SaveChangesAsync();
        public async Task<string> SaveImageAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return "/uploads/" + uniqueFileName; // Trả về đường dẫn ảnh để lưu vào DB
        }
    }
}
