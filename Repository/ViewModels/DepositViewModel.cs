using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class DepositViewModel
    {
        public long number { get; set; }
        public string ReturnUrl { get; set; }
        public string CalleURL { get; set; }
        public string UserID { get; set; }
    }
}
