using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    [Table("Products")]
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public string Image { get; set; }
        [NotMapped]
        public IFormFile ImageContent { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        //constructor
        public Product()
        {
        }
    }
}
