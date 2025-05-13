using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Categories
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid();
        public string? ImageUrl { get; set; }
        public string DisplayOrder { get; set; }
        public string Name { get; set; }
        public int Number { get; set; } = 0;
        public float Commission { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Recipe> Recipes { get; set; }
    }
}
