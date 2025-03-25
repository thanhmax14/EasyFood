using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class RevenuAdmin
    {
        public string Date { get; set; }
        public decimal Commission { get; set; }
        public decimal Deposit { get; set; }
        public decimal Withdraw { get; set; }
        public int NewUsers { get; set; }
    }
}
