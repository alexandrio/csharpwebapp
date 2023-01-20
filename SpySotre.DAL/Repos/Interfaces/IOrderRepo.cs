using System;
using SpyStore.DAL.Repos.Base;
using SpyStore.Models.Entities;
using SpyStore.Models.ViewModels;
namespace SpyStore.DAL.Repos.Interfaces
{
	public interface IOrderRepo:IRepo<Order>
	{
		IList<Order> GetOrderHistory();
		OrderWithDetailsAndProductInfo GetOneWithDetails(int orderId);
	}
}

