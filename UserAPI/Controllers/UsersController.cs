﻿using AutoMapper;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.ViewModels;
using System.Runtime.InteropServices;

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStoreDetailService _storeDetailService;
        private readonly IProductService _productService;
        private readonly IProductVariantService _productVariantService;
        private readonly ICartService _cartService;
        private readonly IProductImageService _productImageService;


        public UsersController(UserManager<AppUser> userManager, IStoreDetailService storeDetailService, IProductService productService, IProductVariantService productVariantService, ICartService cartService, IProductImageService productImageService)
        {
            _userManager = userManager;
            _storeDetailService = storeDetailService;
            _productService = productService;
            _productVariantService = productVariantService;
            _cartService = cartService;
            _productImageService = productImageService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var list = new List<UsersViewModel>();
            var obj = _userManager.Users.ToList();
            if (obj.Any())
            {
                foreach (var user in obj)
                {

                    list.Add(new UsersViewModel
                    {
                        Birthday = user.Birthday,
                        Address = user.Address,
                        img = user.img,
                        RequestSeller = user.RequestSeller,
                        isUpdateProfile = user.isUpdateProfile,
                        ModifyUpdate = user.ModifyUpdate,
                        PhoneNumber = user.PhoneNumber,
                        UserName = user.UserName,
                        Email = user.Email,
                    });

                };

            }

            return Ok(list);
        }
        [HttpGet("View-Profile/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return BadRequest(new { message = "User ID không hợp lệ." });
                }

                var stores = await _storeDetailService.ListAsync(x => x.UserID == id);
                var storeId = stores?.FirstOrDefault()?.ID ?? Guid.Empty; // Kiểm tra null

                var UserModel = new UsersViewModel
                {
                    Birthday = user.Birthday,
                    Address = user.Address,
                    img = user.img,
                    RequestSeller = user.RequestSeller,
                    isUpdateProfile = user.isUpdateProfile,
                    ModifyUpdate = user.ModifyUpdate,
                    PhoneNumber = user.PhoneNumber,
                    UserName = user.UserName,
                    Email = user.Email,
                    StoreDeatilId = storeId
                };

                return Ok(UserModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(string id, [FromBody] UsersViewModel obj)
        {
            if (obj == null)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy user." });
            }

            // Cập nhật thông tin
            user.Birthday = obj.Birthday;
            user.Address = obj.Address;
            user.img = obj.img;
            user.RequestSeller = obj.RequestSeller;
            user.isUpdateProfile = obj.isUpdateProfile;
            user.ModifyUpdate = DateTime.UtcNow;
            user.PhoneNumber = obj.PhoneNumber;
            user.UserName = obj.UserName;
            user.Email = obj.Email;
            /*   _mapper.Map(obj, user);*/

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { message = "Cập nhật user thành công." });
            }

            return BadRequest(new { message = "Cập nhật user thất bại.", errors = result.Errors });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy user." });
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { message = "Xóa user thành công." });
            }

            return BadRequest(new { message = "Xóa user thất bại.", errors = result.Errors });
        }
        [HttpPost("register-seller/{id}")]
        public async Task<IActionResult> RegisterSeller(string id)
        {
            // Tìm user theo Id
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy user." });
            }
            // Cập nhật trạng thái RequestSeller
            user.RequestSeller = "1"; // Đánh dấu yêu cầu đăng ký seller
            user.ModifyUpdate = DateTime.UtcNow; // Cập nhật thời gian sửa đổi

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { message = "Đăng ký seller thành công. Đang chờ phê duyệt." });
            }
            return BadRequest(new { message = "Đăng ký seller thất bại.", errors = result.Errors });
        }
        [HttpGet("ViewCartDetail/{id}")]
        public async Task<IActionResult> ViewCartDetail(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var carts = await _cartService.ListAsync(x => x.UserID == user.Id);
            if (carts == null || !carts.Any())
            {
                return NotFound("Giỏ hàng trống.");
            }

            var products = await _productService.ListAsync();
            var productVariantIds = products.Select(x => x.ID).ToList();
            var productVariants = await _productVariantService.ListAsync(p => productVariantIds.Contains(p.ProductID));
            var result = new List<CartViewModels>(); // Danh sách kết quả

            foreach (var cart in carts)
            {
                var product = products.FirstOrDefault(p => p.ID == cart.ProductID);
                if (product == null)
                {
                    continue; // Bỏ qua nếu không có sản phẩm
                }

                // 🔹 Lấy thông tin biến thể sản phẩm
                var variant = productVariants.FirstOrDefault(v => v.ProductID == product.ID);

                // 🔹 Lấy ảnh sản phẩm
                var productImg = await _productImageService.FindAsync(x => x.ProductID == product.ID);

                var cartItem = new CartViewModels
                {
                    ProductID = cart.ProductID,
                    ProductName = product.Name ?? "Không có tên",
                    quantity = cart.Quantity,
                    price = variant?.Price ?? 0,
                    Subtotal = cart.Quantity * (variant?.Price ?? 0),
                    img = productImg?.ImageUrl ?? "/images/default.jpg", // Nếu không có ảnh thì lấy ảnh mặc định
                    Stock = variant?.Stock ?? 0 // 🔹 Lấy số lượng trong kho từ ProductVariant
                };

                result.Add(cartItem);
            }

            return Ok(result); // Trả về danh sách giỏ hàng
        }

        [HttpPost("AddCart")]
        public async Task<IActionResult> AddCart(Guid productId, int quantity, CartItem obj)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var product = await _productService.FindAsync(x => x.ID == productId);
                if(product == null)
                {
                    return BadRequest(new { message = "Product not found" });
                }
                var cartItem = await _cartService.FindAsync(x => x.UserID == user.Id && x.ProductID == productId);
                if (cartItem != null)
                {
                    cartItem.Quantity += quantity;
                    await _cartService.UpdateAsync(cartItem);
                } else
                {
                    var newCart = new Cart
                    {
                        UserID = user.Id,  // Gán ID người dùng
                        ProductID = productId,
                        Quantity = quantity
                    };
                    await _cartService.AddAsync(newCart);
                }
                return Ok("Add Cart Success!");

            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Lỗi server: {ex.Message}");

            }
        }
        [HttpPost("UpdateCart/{id}")]
        public async Task<IActionResult> UpdateCart([FromBody] CartItem obj)
        {
            var carItem = await _cartService.FindAsync(x => x.ProductID == obj.ProductID);
            if(carItem == null)
            {
                return BadRequest(new { message = "Cart not found" });
            }
            carItem.Quantity = obj.quantity;
            await _cartService.UpdateAsync(carItem);
            await _cartService.SaveChangesAsync();
            var product = await _productService.FindAsync(x => x.ID == obj.ProductID);
            var Productvar = await _productVariantService.FindAsync(x => x.ProductID == product.ID);
            decimal subtotal = carItem.Quantity * (Productvar?.Price ?? 0);
            return Ok(new { success = true, subtotal = subtotal });

        }


    }
}
