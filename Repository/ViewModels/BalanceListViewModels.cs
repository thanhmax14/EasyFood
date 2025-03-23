using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class BalanceListViewModels
    {
        public int No { get; set; } = 0;
        public decimal Before { get; set; } = 0;
        public decimal Change { get; set; } = 0;
        public decimal After { get; set; } = 0;
        public string Types { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public string Invoice { get; set; }

    }
}
