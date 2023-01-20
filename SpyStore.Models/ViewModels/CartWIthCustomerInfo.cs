using System;
using SpyStore.Models.Entities;
using System.Collections.Generic;

namespace SpyStore.Models.ViewModels
{
	public class CartWIthCustomerInfo
	{

		public Customer Customer { get; set; }
		public IList<CartRecordWithProductInfo> CartRecords { get; set; } =new List<CartRecordWithProductInfo>();
	}
}

