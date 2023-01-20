using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SpyStore.Models.Entities.Base;
using Newtonsoft.Json;

namespace SpyStore.Models.Entities
{
    [Table("ShoppingCartRecords", Schema ="Store")]
    public class ShoppingCartRecord:ShoppingCartRecordBase
    {
        [JsonIgnore]
        [ForeignKey(nameof(CustomerId))]
        public Customer CustomerNavigation { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(ProductId))]
        public Product ProductNavigation { get; set; }
    }
}