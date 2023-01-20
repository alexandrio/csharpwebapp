using System;
using Microsoft.EntityFrameworkCore;
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
	}
}

