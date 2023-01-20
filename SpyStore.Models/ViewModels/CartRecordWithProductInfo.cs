using System;
using SpyStore.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;
namespace SpyStore.Models.ViewModels
{
	public class CartRecordWithProductInfo:ShoppingCartRecordBase
	{

		// esta propiedad se agrega para ocultar la propiedad id definida en la clase base
		//entities asigned to dbquery<t> no pueden tener una primary key
		public new int Id { get; set; }

		public CartRecordWithProductInfo()
		{
		}

        //Not supported at this time
        //public ProductDetails Details { get; set; }
        public string Description { get; set; }
        [Display(Name = "Model Number")]
        public string ModelNumber { get; set; }
        [Display(Name = "Name")]
        public string ModelName { get; set; }
        public string ProductImage { get; set; }
        public string ProductImageLarge { get; set; }
        public string ProductImageThumb { get; set; }
        [Display(Name = "In Stock")]
        public int UnitsInStock { get; set; }
        [Display(Name = "Price"), DataType(DataType.Currency)]
        public decimal CurrentPrice { get; set; }
        public int CategoryId { get; set; }
        [Display(Name = "Category")]
        public string CategoryName { get; set; }

    }
}

