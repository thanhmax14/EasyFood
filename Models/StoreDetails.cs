using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
    {
        public class StoreDetails
        {
            [Key]
            public Guid ID { get; set; } = Guid.NewGuid();

            [ForeignKey("AppUser")]
            public string UserID { get; set; }

            public string Name { get; set; } = default!;
            public DateTime CreatedDate { get; set; } = DateTime.Now;
            public DateTime? ModifiedDate { get; set; }
            public string? Description { get; set; }
            public string? Address { get; set; }
            public string? Call { get; set; }

            public virtual AppUser AppUser { get; set; }
            public ICollection<Product> Products { get; set; }
    }
}
