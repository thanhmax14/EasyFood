using Microsoft.EntityFrameworkCore.Storage;
using Models.DBContext;

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

        public async Task<bool> ExecuteInTransactionAsync(Func<Task> action)
        {
            try
            {
                await BeginTransactionAsync();
                await action();
                await CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    await RollbackAsync();
                }
                catch (Exception rollbackEx)
                {
                    throw new Exception("Lỗi rollback giao dịch!", rollbackEx);
                }
                throw new Exception("Lỗi trong giao dịch!", ex);
            }
        }


        public void Dispose()
        {
            DisposeTransactionAsync().Wait();
        }

    }
}
