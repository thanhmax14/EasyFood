using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class AppUser:IdentityUser
    {
        public DateTime? CreateDate { get; set; }
        public decimal tien { get; set; } = 0;
        public string? Fullname { get; set; }
        public DateTime lastAssces { get; set; } = DateTime.Now;
    }
}
