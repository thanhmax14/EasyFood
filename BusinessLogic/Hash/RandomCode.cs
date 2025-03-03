using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Hash
{
    public static class RandomCode
    {
        /// <summary>
        /// Tạo mã duy nhất với tiền tố và chuỗi ngẫu nhiên.
        /// </summary>
        /// <param name="prefixLength">Độ dài tiền tố ngẫu nhiên.</param>
        /// <param name="randomLength">Độ dài phần ngẫu nhiên (không tính tiền tố và timestamp).</param>
        /// <returns>Mã duy nhất.</returns>
        public static string GenerateUniqueCode(int prefixLength = 4, int randomLength = 6)
        {
            var randomPrefix = GenerateRandomString(prefixLength);
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // Mili giây
            var randomPart = GenerateRandomString(randomLength);
            /* var guidPart = Guid.NewGuid().ToString("N"); // Thêm GUID để đảm bảo tính duy nhất*/
            return $"{randomPrefix}{timestamp}{randomPart}";
        }


        /// <summary>
        /// Tạo chuỗi ngẫu nhiên với các ký tự chữ và số.
        /// </summary>
        /// <param name="length">Độ dài chuỗi ngẫu nhiên.</param>
        /// <returns>Chuỗi ngẫu nhiên.</returns>
        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var randomBytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            var result = new StringBuilder(length);
            foreach (var b in randomBytes)
            {
                result.Append(chars[b % chars.Length]);
            }

            return result.ToString();
        }
       

        public static string FormatCurrency(decimal amount)
        {
            // Kiểm tra xem số có phải là số nguyên không
            if (amount == Math.Floor(amount))
            {
                // Nếu là số nguyên, hiển thị mà không có phần thập phân
                return string.Format("{0:#,0} đ", amount);
            }
            else
            {
                // Nếu là số thập phân, hiển thị tối đa 2 chữ số sau dấu thập phân
                return string.Format("{0:#,0.##} đ", amount);
            }
        }
        public static string FormatIntCurrency(int amount)
        {
            return string.Format("{0:#,0} đ", amount);
        }

        public static int GenerateOrderCode()
        {
            string guidPart = Guid.NewGuid().ToString("N").Substring(0, 6); // Lấy 6 ký tự đầu của Guid
            return int.Parse(guidPart, System.Globalization.NumberStyles.HexNumber) % 1000000;
        }
    }
}
