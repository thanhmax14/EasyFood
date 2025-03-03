using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Review
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public string? Cmt { get; set; }
        public DateTime Datecmt { get; set; } = DateTime.Now;

        public string? Relay { get; set; }
        public DateTime DateRelay { get; set; } = DateTime.Now;
        public bool Status { get; set; } = false;
        public int Rating { get; set; } = 5;

        public string UserID { get; set; }
        [ForeignKey("Product")]
        public Guid ProductID { get; set; }
        public virtual Product Product { get; set; }
        public AppUser AppUser { get; set; }
    }
}
