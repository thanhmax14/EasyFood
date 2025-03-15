using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class UsersViewModel
    {
     /*   public DateTime? joinin { get; set; } = DateTime.Now;
        public DateTime lastAssces { get; set; } = DateTime.Now;
        public string? FirstName { get; set; } = default;
        public string? LastName { get; set; } = default;*/
        public DateTime? Birthday { get; set; }
        public string? Address { get; set; } = default;
        public string? RequestSeller { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? img { get; set; } = "";
        public bool isUpdateProfile { get; set; } = false;
        public bool IsBanByadmin { get; set; } = false;

        /*        public bool IsBanByadmin { get; set; } = false;
        */
        public DateTime? ModifyUpdate { get; set; } = DateTime.Now;
        public Guid StoreDeatilId { get; set; }

    }
}
