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
    }
}
