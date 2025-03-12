using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class AdminViewModel
    {
        public string? Address { get; set; } = default;
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? Birthday { get; set; }

    }
}
