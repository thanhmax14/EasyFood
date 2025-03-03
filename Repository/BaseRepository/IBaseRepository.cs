using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.BaseRepository
{
    public interface IBaseRepository<T> where T : class
    {
        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>The <see cref="IQueryable" />.</returns>
        IQueryable<T> GetAll();
        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="T" />.</returns>
        T GetById(Guid id);
        /// <summary>
        /// Gets the asynchronous by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        Task<T> GetAsyncById(Guid id);
        /// <summary>
        /// Finds the specified match.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>T.</returns>
        T Find(Expression<Func<T, bool>> match);
        /// <summary>
        /// Finds the asynchronous.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        Task<T> FindAsync(Expression<Func<T, bool>> match);
        /// <summary>
        /// The get by id as detached.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="T" />.</returns>
        T GetByIdAsDetached(Guid id);
        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <summary>
        /// Add new entity object asynchronously.
        /// </summary>
        /// <param name="entity">The entity object</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Update the entity object asynchronously.
        /// </summary>
        /// <param name="entity">The entity object</param>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Delete the entity object asynchronously.
        /// </summary>
        /// <param name="entity">The entity object</param>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Delete the entity object asynchronously by id.
        /// </summary>
        /// <param name="id">The id.</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Check if the entity exists asynchronously.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>True if exists, otherwise false.</returns>
        Task<bool> ExistsAsync(Guid id);
        /// <summary>
        /// Save changes.
        /// </summary>
        /// <returns>System.Int32.</returns>
        int SaveChanges();
        /// <summary>
        /// Saves the changes asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        Task<int> SaveChangesAsync();
        /// <summary>
        /// Counts this instance.
        /// </summary>
        /// <returns>System.Int32.</returns>
        int Count();

        /// <summary>
        /// Tìm kiếm linh hoạt theo điều kiện.
        /// </summary>
        /// <param name="predicate">Điều kiện tìm kiếm (lambda expression).</param>
        /// <returns>Danh sách kết quả hoặc null.</returns>
        Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Counts the asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        Task<int> CountAsync();
        /// <summary>
        /// Lists the asynchronous.
        /// </summary>
        /// <returns>Task&lt;IEnumerable&lt;T&gt;&gt;.</returns>
        Task<IEnumerable<T>> ListAsync();
        /// <summary>
        /// Lists the asynchronous.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns>Task&lt;IEnumerable&lt;T&gt;&gt;.</returns>
        Task<IEnumerable<T>> ListAsync(
        Expression<Func<T, bool>> filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        /// <summary>
        /// Lists the asynchronous.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns>Task&lt;IEnumerable&lt;T&gt;&gt;.</returns>
        Task<IEnumerable<T>> ListAsync(
        Expression<Func<T, bool>> filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> includeProperties = null);
    }
}
