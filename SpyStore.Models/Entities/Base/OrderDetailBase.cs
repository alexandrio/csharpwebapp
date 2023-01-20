using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SpyStore.Models.Entities.Base
{
	public class OrderDetailBase:EntityBase
	{
		[Required]
		public int OrderId { get; set; }
		[Required]
		public int ProductId { get; set; }
		[Required]
		public int Quantity { get; set; }

		[DataType(DataType.Currency), Display(Name ="Total")]
		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public decimal LineItemTotal { get; set; }
	
		[Required, DataType(DataType.Currency), Display(Name ="UnitCost")]
		public decimal UnitCost { get; set; }
		public OrderDetailBase()
		{
		}
	}
}

