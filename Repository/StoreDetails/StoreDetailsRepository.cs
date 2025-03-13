﻿using Models.DBContext;
using Models;
using Repository.BaseRepository;
using Repository.Categorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repository.ViewModels;

namespace Repository.StoreDetails
{
    public class StoreDetailsRepository : BaseRepository<Models.StoreDetails>, IStoreDetailsRepository
    {
        private readonly EasyFoodDbContext _context;
        public StoreDetailsRepository(EasyFoodDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<bool> IsUserSellerAsync(string userId)
        {
            return await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userId &&
                                _context.Roles.Any(r => r.Id == ur.RoleId && r.Name.ToLower() == "seller"));
        }

        public async Task<bool> AddStoreAsync(Models.StoreDetails store)
        {
            try
            {
                await _context.StoreDetails.AddAsync(store);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<IEnumerable<Models.StoreDetails>> GetAllStoresAsync()
        {
            return await _context.StoreDetails.ToListAsync();
        }

        public async Task<Models.StoreDetails?> GetStoreByIdAsync(Guid storeId)
        {
            return await _context.StoreDetails.FirstOrDefaultAsync(s => s.ID == storeId);
        }
        public async Task<Models.StoreDetails> GetByIdAsync(Guid id)
        {
            return await _context.StoreDetails.FindAsync(id);
        }

        public async Task UpdateAsync(Models.StoreDetails storeDetails)
        {
            _context.StoreDetails.Update(storeDetails);
            await _context.SaveChangesAsync();
        }
        public async Task<List<StoreViewModel>> GetInactiveStoresAsync()
        {
            var stores = await _context.StoreDetails
                .Join(
                    _context.Users,
                    s => s.UserID,
                    u => u.Id,
                    (s, u) => new StoreViewModel
                    {
                        ID = s.ID,
                        Name = s.Name,
                        CreatedDate = s.CreatedDate,
                        ShortDescriptions = s.ShortDescriptions ?? "No description available",
                        Address = s.Address,
                        Phone = s.Phone,
                        Img = !string.IsNullOrEmpty(s.Img) ? s.Img : "default-store.png", // Chỉ lưu tên file
                        UserName = u.UserName,
                        IsActive = s.IsActive,
                    }
                )
                .ToListAsync();

            return stores;
        }
        public async Task<List<StoreViewModel>> GetActiveStoresAsync()
        {
            var stores = await _context.StoreDetails
                .Join(
                    _context.Users,
                    s => s.UserID,
                    u => u.Id,
                    (s, u) => new StoreViewModel
                    {
                        ID = s.ID,
                        Name = s.Name,
                        CreatedDate = s.CreatedDate,
                        ShortDescriptions = s.ShortDescriptions ?? "No description available",
                        Address = s.Address,
                        Phone = s.Phone,
                        Img = !string.IsNullOrEmpty(s.Img) ? s.Img : "default-store.png", // Chỉ lưu tên file
                        UserName = u.UserName
                    }
                )
                .ToListAsync();

            return stores;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<bool> UpdateStoreAsync(Models.StoreDetails store)
        {
            try
            {
                _context.StoreDetails.Update(store);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Ghi log nếu cần
                Console.WriteLine($"Error updating store: {ex.Message}");
                return false;
            }
        }
    }
}
