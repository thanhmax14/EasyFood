using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Review
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid();
        public string? Comment { get; set; }
        public DateTime CommentDate { get; set; } = DateTime.Now;

        public string? Reply { get; set; }
        public DateTime? ReplyDate { get; set; } = DateTime.Now;
        public bool Status { get; set; } = false;
        public int Rating { get; set; } = 5;
        [ForeignKey("AppUser")]
        public string UserID { get; set; }
        [ForeignKey("Product")]
        public Guid ProductID { get; set; }
        public virtual Product Product { get; set; }
        public AppUser AppUser { get; set; }
    }
}
