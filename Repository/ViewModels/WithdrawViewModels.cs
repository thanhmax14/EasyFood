using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class WithdrawViewModels
    {
        public decimal amount { get; set; }
        public string  AccountName { get; set; }
        public string accountNumber { get; set; }
        public string BankName { get; set; }
        public string UserID { get; set; }

    }
}
