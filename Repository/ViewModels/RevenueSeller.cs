using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class RevenueSeller
    {
        public int PROCESSING { get; set; }
        public int Success { get; set; }
        public int Failed { get; set; }
        public decimal Earnings { get; set; }
        public int Orders { get; set; }
        public string Date { get; set; }
      
    }
}
