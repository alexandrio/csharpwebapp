using System;
using Microsoft.EntityFrameworkCore;
using SpyStore.DAL.EfStructures;
using SpyStore.DAL.Repos.Base;
using SpyStore.DAL.Repos.Interfaces;
using SpyStore.Models.Entities;

namespace SpyStore.DAL.Repos
{
	public class ProductRepo :RepoBase<Product> , IProductRepo
	{
		public ProductRepo(StoreContext context) : base(context)
		{
		}
		internal ProductRepo(DbContextOptions<StoreContext> options) :base(options)
		{

		}

		public override IEnumerable<Product> GetAll() => base.GetAll(x => x.Details.ModelName);

		public IList<Product> GetProductsForCategory(int id) => Table.Where(p => p.CategoryId == id)
																		.Include(p => p.CategoryNavigation)
																		.OrderBy(x => x.Details.ModelName)
																		.ToList();

		public IList<Product> GetFeaturedWithCategoryName() => Table.Where(p => p.IsFeatured)
																.Include(p => p.CategoryNavigation)
																.OrderBy(x => x.Details.ModelName)
																.ToList();

		public Product GetOneWithCategoryName(int id) => Table.Where(p => p.Id == id)
																.Include(p => p.CategoryNavigation)
																.FirstOrDefault();

		public IList<Product> Search(string searchString) => Table.Where(p => EF.Functions.Like(p.Details.Description, $"%{searchString}%") || EF.Functions.Like(p.Details.ModelName, $"%{searchString}%"))
																	.Include(p => p.CategoryNavigation)
																	.OrderBy(x => x.Details.ModelName)
																	.ToList();

	}
}

