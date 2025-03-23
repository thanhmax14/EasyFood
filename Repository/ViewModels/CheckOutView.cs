using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class CheckOutView
    {
        public List<ListItems> itemCheck { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
/*        public string firstName { get; set; }
        public string firstName { get; set; }*/
    }
    public class ListItems
    {
        public string ItemName { get; set; }
        public string ItemImage { get; set; }
        public int ItemQuantity { get; set; }
        public decimal ItemPrice { get; set; }
        public Guid productID { get; set; }
    }
}
