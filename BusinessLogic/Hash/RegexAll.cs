using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessLogic.Hash
{
    public static class RegexAll
    {
        public static bool ContainsAmpersand(string input)
        {
            try
            {
                if (input == null)
                    return false;

                return input.Contains("&");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                return false; 
            }
        }


        public static string? ExtractPayosLink(string? text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                    return null;

                string pattern = @"https?:\/\/pay\.payos\.vn\/web\/[a-zA-Z0-9]+\/?";
                Match match = Regex.Match(text, pattern);

                return match.Success ? match.Value : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi xảy ra: {ex.Message}");
                return null;
            }
        }
    }
}
