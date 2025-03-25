using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class GetRevenueTotal
    {
        public decimal TotalCommission { get; set; }
        public int TotalStores { get; set; }
        public int TotalSellers { get; set; }
        public int TotalUsers { get; set; }
    }
}
