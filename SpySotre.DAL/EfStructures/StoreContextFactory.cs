using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace SpyStore.DAL.EfStructures
{
	public class StoreContextFactory:IDesignTimeDbContextFactory<StoreContext>
	{
		public StoreContextFactory()
		{
		}

        public StoreContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<StoreContext>();
            var connectionString = @"Server=WINMINI,1433;Database=SpyStore22;User ID=sa;Password=Sistemas2018;MultipleActiveResultSets=true;TrustServerCertificate=true";

            optionsBuilder.UseSqlServer(connectionString, options => options.EnableRetryOnFailure());
            // a partir de .net core 3.0 se elimino y ahora genera una excepcion directa
            //optionsBuilder.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));
            Console.WriteLine(connectionString);
            return new StoreContext(optionsBuilder.Options);
        }
    }
}

