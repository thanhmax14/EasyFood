using System.ComponentModel.DataAnnotations.Schema;
using Models;

namespace Repository.ViewModels
{
    public class ReivewViewModel
    {
        public Guid ID { get; set; }
        public string? Cmt { get; set; }
        public DateTime Datecmt { get; set; } = DateTime.Now;

        public string? Relay { get; set; }
<<<<<<< Updated upstream
        public DateTime DateRelay { get; set; } = DateTime.Now;
=======
        public DateTime? DateRelay { get; set; } = DateTime.Now;
        //1 (true) → ẩn
        //0 (false) → hiện
>>>>>>> Stashed changes
        public bool Status { get; set; } = false;
        public int Rating { get; set; } = 5;

        public string UserID { get; set; }
        [ForeignKey("Product")]
        public Guid ProductID { get; set; }
        public virtual Product Product { get; set; }
        public AppUser AppUser { get; set; }
        //public string OrderCode { get; set; }
        //public string Username { get; set; }
        //public string ProductName { get; set; }

<<<<<<< Updated upstream
=======
        public string? Username { get; set; }
        public string? ProductName { get; set; }

        public Guid? StoreId { get; set; }

        public string? StoreName { get; set; }
>>>>>>> Stashed changes

    }
}
