using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class InvoiceViewModels
    {
        public string? orderCoce  { get; set; }
        public DateTime? invoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? paymentMethod { get; set; }
        public string? status { get; set; }
        public string?  NameUse { get; set; }
        public string? AddressUse { get; set; }
        public string? phoneUser { get; set; }
        public string? emailUser { get; set; }
        public decimal? tax { get; set; } = 0;
        public decimal? discountVocher { get; set; }
        public string? vocherName { get; set; }
        public List<ItemInvoice> itemList { get; set; } = new List<ItemInvoice>();
    }
}
