using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SpyStore.DAL.EfStructures;
using SpyStore.DAL.Repos.Base;
using SpyStore.DAL.Repos.Interfaces;
using SpyStore.Models.Entities;


namespace SpyStore.DAL.Repos
{
	public class CategoryRepo: RepoBase<Category>, ICategoryRepo
	{
		public CategoryRepo(StoreContext context):base(context)
		{
		}

		internal CategoryRepo(DbContextOptions<StoreContext> options):base(options)
		{

		}

		public override IEnumerable<Category> GetAll() => base.GetAll(x => x.CategoryName);
	}
}

