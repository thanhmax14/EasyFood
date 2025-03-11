using AutoMapper;
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

        public UsersController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return BadRequest(new { message = "User ID không hợp lệ." });
            }
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
            };
            return Ok(UserModel);
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

    }
}
