using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SpyStore.DAL.Initialization;
using SpyStore.DAL.EfStructures;
using SpyStore.Models.Entities;

namespace SpyStore.DAL.Inicialization
{
	public static class SampleDataInitializer
	{
		public static void DropAndCreateDatabase(StoreContext context)
		{
			context.Database.EnsureDeleted();
			context.Database.Migrate();
		}

		internal static void ResetIdentity(StoreContext context)
		{
			var tables = new[] { "Categories", "Customers", "OrderDetails", "Orders", "Products", "ShoppingCartRecords" };



//            Note EF Core parameters queries when using C# string interpolation. If string
//interpolation is used before the call to ExecuteSqlCommand(as is done in Listing 4 - 23)
//or a call to FromSql, EF Core will raise a warning for possible SQL injection risk.The
//#pragma statements in the listing disable the warnings.

            foreach (var itm in tables)
			{
				var rawSqlString = $"DBCC CHECKIDENT (\"Store.{itm}\",RESEED,0);";
				// en el libro viene como ExecuteSqlCommand
#pragma warning disable EF1000
				context.Database.ExecuteSqlRaw(rawSqlString);
#pragma warning restore EF1000
			}
		}

		public static void ClearData(StoreContext context)
		{
			context.Database.ExecuteSqlRaw("Delete from Store.Categories");
			context.Database.ExecuteSqlRaw("Delete from Store.Customers");
			ResetIdentity(context);
		}

		internal static void SeedData(StoreContext context)
		{
			try
			{
				if(!context.Categories.Any())
				{
					context.Categories.AddRange(SampleData.GetCategories());
					context.SaveChanges();
				}

				if (!context.Customers.Any())
				{
					var prod1 = context.Categories
						.Include(c => c.Products).FirstOrDefault()?
						.Products.Skip(3).FirstOrDefault();

					var prod2 = context.Categories.Skip(2)
						.Include(c => c.Products).FirstOrDefault()?
						.Products.Skip(2).FirstOrDefault();

					var prod3 = context.Categories.Skip(5)
						.Include(c => c.Products).FirstOrDefault()?
						.Products.Skip(1).FirstOrDefault();

					var prod4 = context.Categories.Skip(2)
						.Include(c => c.Products).FirstOrDefault()?
						.Products.Skip(1).FirstOrDefault();

					context.Customers.AddRange(SampleData.GetAllCustomerRecords(
						new List<Product> { prod1,prod2,prod3,prod4}
						));

					var total = context.Customers.Count();

					context.SaveChanges();
				}
			}catch(Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		public static void InitializeData(StoreContext context)
		{
			context.Database.Migrate();
			ClearData(context);
			SeedData(context);

		}

	}
}

