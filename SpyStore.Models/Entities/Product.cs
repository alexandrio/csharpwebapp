using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SpyStore.Models.Entities.Base;

namespace SpyStore.Models.Entities
{
    [Table("Products", Schema ="Store")]
    public class Product:EntityBase
    {

        public ProductDetails Details { get; set; } = new ProductDetails();
        public bool IsFeatured { get; set; }
        [DataType(DataType.Currency)]
        public decimal UnitCost { get; set; }
        [DataType(DataType.Currency)]
        public decimal CurrentPrice { get; set; }
        public int UnitsInStock { get; set; }
        [Required]
        public int CategoryId { get; set; }


        [InverseProperty(nameof(ShoppingCartRecord.ProductNavigation))]
        public List<ShoppingCartRecord> ShoppingCartRecords = new List<ShoppingCartRecord>();

        [InverseProperty(nameof(OrderDetail.ProductNavigation))]
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        [JsonIgnore]
        [ForeignKey(nameof(CategoryId))]
        public Category CategoryNavigation { get; set; }

        [NotMapped]
        public string CategoryName => CategoryNavigation?.CategoryName;
    }
}