using System;
using SpyStore.Models.Entities;
using SpyStore.Models.Entities.Base;
using AutoMapper;
namespace SpyStore.Models.ViewModels
{
	public class OrderWithDetailsAndProductInfo:OrderBase
	{
		private static readonly MapperConfiguration _mapperCfg;

		static OrderWithDetailsAndProductInfo()
		{
			_mapperCfg = new MapperConfiguration(cfg =>
			{

				cfg.CreateMap<Order, OrderWithDetailsAndProductInfo>().ForMember(record => record.OrderDetails, y => y.Ignore());
			});

		}

		public static OrderWithDetailsAndProductInfo Create(Order order,Customer customer, IEnumerable<OrderDetailWithProductInfo> details)
		{
			var viewModel = _mapperCfg.CreateMapper().Map<OrderWithDetailsAndProductInfo>(order);
			viewModel.OrderDetails = details.ToList();
			viewModel.Customer = customer;
			return viewModel;
		}

		public Customer Customer { get; set; }

		public IList<OrderDetailWithProductInfo> OrderDetails { get; set; }
	}
}

