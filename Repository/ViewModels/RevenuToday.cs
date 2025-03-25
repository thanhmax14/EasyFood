using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class RevenuToday
    {
        public decimal Earnings { get; set; }
        public int Orders { get; set; }
        public int Refunds { get; set; }
        public decimal EarningsChange { get; set; }
        public decimal CompletionRate { get; set; }
    }
}
