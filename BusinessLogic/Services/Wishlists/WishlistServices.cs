using AutoMapper;
using Models;
using Repository.Wishlists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Wishlists
{
    public class WishlistServices:IWishlistServices
    {
        private readonly IWishlistRepository _repository;
        private readonly IMapper _mapper;

        public WishlistServices(IWishlistRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IQueryable<Wishlist> GetAll() => _repository.GetAll();

        public Wishlist GetById(Guid id) => _repository.GetById(id);

        public async Task<Wishlist> GetAsyncById(Guid id) => await _repository.GetAsyncById(id);

        public Wishlist Find(Expression<Func<Wishlist, bool>> match) => _repository.Find(match);

        public async Task<Wishlist> FindAsync(Expression<Func<Wishlist, bool>> match) => await _repository.FindAsync(match);

        public async Task AddAsync(Wishlist entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(Wishlist entity) => await _repository.UpdateAsync(entity);

        public async Task DeleteAsync(Wishlist entity) => await _repository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _repository.ExistsAsync(id);
        public int Count() => _repository.Count();

        public async Task<int> CountAsync() => await _repository.CountAsync();

        public async Task<IEnumerable<Wishlist>> ListAsync() => await _repository.ListAsync();

        public async Task<IEnumerable<Wishlist>> ListAsync(
            Expression<Func<Wishlist, bool>> filter = null,
            Func<IQueryable<Wishlist>, IOrderedQueryable<Wishlist>> orderBy = null,
            Func<IQueryable<Wishlist>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Wishlist, object>> includeProperties = null) =>
            await _repository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _repository.SaveChangesAsync();
    }
}
