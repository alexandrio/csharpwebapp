using System;
using SpyStore.DAL.Repos.Base;
using SpyStore.Models.Entities;
namespace SpyStore.DAL.Repos.Interfaces
{
	public interface IProductRepo :IRepo<Product>
	{
		IList<Product> Search(string searchString);
		IList<Product> GetProductsForCategory(int Id);
		IList<Product> GetFeaturedWithCategoryName();
		Product GetOneWithCategoryName(int id);

	}
}

