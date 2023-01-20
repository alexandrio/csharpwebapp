using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using SpyStore.DAL.EfStructures;
using SpyStore.DAL.Repos.Base;
using SpyStore.DAL.Repos.Interfaces;
using SpyStore.Models.Entities;
using SpyStore.Models.ViewModels;

namespace SpyStore.DAL.Repos
{
	public class OrderDetailRepo : RepoBase<OrderDetail>, IOrderDetailRepo
	{
		public OrderDetailRepo(StoreContext context):base(context)
		{
		}

		internal OrderDetailRepo(DbContextOptions<StoreContext> options) : base(options)
		{

		}

		public IEnumerable<OrderDetailWithProductInfo> GetOrderDetailWithProductInfoForOrder(int orderId) =>
				Context
						.OrderDetailWithProductInfos
						.Where(x => x.OrderId == orderId)
						.OrderBy(x => x.ModelName);
	}
}

