using Microsoft.EntityFrameworkCore.Storage;
using Models.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.BalanceChange
{
    public class ManageTransaction : IDisposable
    {
        private readonly EasyFoodDbContext _dbContext;
        private IDbContextTransaction _transaction;

        public ManageTransaction(EasyFoodDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Bắt đầu một transaction mới.
        /// </summary>
        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
            {
                _transaction = await _dbContext.Database.BeginTransactionAsync();
            }
        }

        /// <summary>
        /// Commit transaction hiện tại.
        /// </summary>
        public async Task CommitAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await DisposeTransactionAsync();
            }
        }

        /// <summary>
        /// Rollback transaction hiện tại.
        /// </summary>
        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await DisposeTransactionAsync();
            }
        }

        /// <summary>
        /// Hủy transaction hiện tại.
        /// </summary>
        private async Task DisposeTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Thực thi hành động bên trong transaction một cách an toàn.
        /// </summary>
        /// <param name="action">Hành động cần thực thi.</param>
        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            try
            {
                await BeginTransactionAsync();
                await action();
                await CommitAsync();
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
        }

        public void Dispose()
        {
            DisposeTransactionAsync().Wait();
        }

    }
}
