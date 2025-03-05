using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class RecipeReview
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid();
        public string Cmt { get; set; }
        public string? Relay { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? DateRelay { get; set; }
        public bool IsActive { get; set; } = true;
        [ForeignKey("Recipe")]
        public Guid RecipeID { get; set; }

        [ForeignKey("AppUser")]
        public string UserID { get; set; }

        public virtual Recipe Recipe { get; set; }
        public virtual AppUser AppUser { get; set; }
    }
}
