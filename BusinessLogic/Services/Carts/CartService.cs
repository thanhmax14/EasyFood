using AutoMapper;
using Microsoft.AspNetCore.Http;
using Models;
using Newtonsoft.Json;
using Repository.Carts;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Carts
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartService(ICartRepository repository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<Cart> GetAll() => _repository.GetAll();

        public Cart GetById(Guid id) => _repository.GetById(id);

        public async Task<Cart> GetAsyncById(Guid id) => await _repository.GetAsyncById(id);

        public Cart Find(Expression<Func<Cart, bool>> match) => _repository.Find(match);

        public async Task<Cart> FindAsync(Expression<Func<Cart, bool>> match) => await _repository.FindAsync(match);

        public async Task AddAsync(Cart entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(Cart entity) => await _repository.UpdateAsync(entity);

        public async Task DeleteAsync(Cart entity) => await _repository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _repository.ExistsAsync(id);
        public int Count() => _repository.Count();

        public async Task<int> CountAsync() => await _repository.CountAsync();

        public async Task<IEnumerable<Cart>> ListAsync() => await _repository.ListAsync();

        public async Task<IEnumerable<Cart>> ListAsync(
            Expression<Func<Cart, bool>> filter = null,
            Func<IQueryable<Cart>, IOrderedQueryable<Cart>> orderBy = null,
            Func<IQueryable<Cart>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Cart, object>> includeProperties = null) =>
            await _repository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _repository.SaveChangesAsync();


        public List<CartItem> GetCartFromSession()
        {
            var cart = _httpContextAccessor.HttpContext.Session.GetString("Cart");
            return string.IsNullOrEmpty(cart) ? new List<CartItem>() : JsonConvert.DeserializeObject<List<CartItem>>(cart);
        }

        public void SaveCartToSession(List<CartItem> cart)
        {
            if (cart == null)
            {
                _httpContextAccessor.HttpContext.Session.Remove("Cart");
            }
            else
            {
                var jsonSettings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                _httpContextAccessor.HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart, jsonSettings));
            }
        }


    }
}
