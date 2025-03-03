using AutoMapper;
using Models;
using Repository.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Reviews
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _repository;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IQueryable<Review> GetAll() => _repository.GetAll();

        public Review GetById(Guid id) => _repository.GetById(id);

        public async Task<Review> GetAsyncById(Guid id) => await _repository.GetAsyncById(id);

        public Review Find(Expression<Func<Review, bool>> match) => _repository.Find(match);

        public async Task<Review> FindAsync(Expression<Func<Review, bool>> match) => await _repository.FindAsync(match);

        public async Task AddAsync(Review entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(Review entity) => await _repository.UpdateAsync(entity);

        public async Task DeleteAsync(Review entity) => await _repository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _repository.ExistsAsync(id);
        public int Count() => _repository.Count();

        public async Task<int> CountAsync() => await _repository.CountAsync();

        public async Task<IEnumerable<Review>> ListAsync() => await _repository.ListAsync();

        public async Task<IEnumerable<Review>> ListAsync(
            Expression<Func<Review, bool>> filter = null,
            Func<IQueryable<Review>, IOrderedQueryable<Review>> orderBy = null,
            Func<IQueryable<Review>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Review, object>> includeProperties = null) =>
            await _repository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _repository.SaveChangesAsync();
    }
}
