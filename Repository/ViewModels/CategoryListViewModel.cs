using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class CategoryListViewModel
    {
        public Guid ID { get; set; }
        public string Img { get; set; }
        public string Name { get; set; }
        public int Number { get; set; } // Thứ tự hiển thị
        public decimal Commission { get; set; } // % hoa hồng
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
