using System;
using Microsoft.EntityFrameworkCore;
using SpyStore.Models;
using SpyStore.Models.Entities;
using SpyStore.Models.Entities.Base;

using SpyStore.Models.ViewModels;

namespace SpyStore.DAL.EfStructures
{
	public class StoreContext : DbContext
	{
		public int CustomerId { get; set; }

		public DbSet<Category> Categories { get; set; }
		public DbSet<Customer> Customers { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderDetail> OrderDetails { get; set;}
		public DbSet<Product> Products { get; set; }
		public DbSet<ShoppingCartRecord> ShoppingCartRecords { get; set; }


		public DbSet<CartRecordWithProductInfo> CartRecordWithProductInfos { get; set; }
        public DbSet<OrderDetailWithProductInfo> OrderDetailWithProductInfos { get; set; }


        public StoreContext()
		{
		}

        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
        }

		[DbFunction("GetOrderTotal",Schema ="Store")]
		public static int GetOrderTotal(int OrderId)
		{
			throw new Exception();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Customer>(entity => {
				// en el libro originalmente tenia HasName pero se reemplaza con HasDatabaseName
				entity.HasIndex(e => e.EmailAddress).HasDatabaseName("IX_Customers").IsUnique();
			});

			modelBuilder.Entity<Order>(entity=>
			{
				entity.Property(e=> e.Orderdate).HasColumnType("datetime").HasDefaultValueSql("getdate()");
				entity.Property(e => e.ShipDate).HasColumnType("datetime").HasDefaultValueSql("getdate()");
				entity.Property(e => e.OrderTotal).HasColumnType("money").HasComputedColumnSql("Store.GetOrderTotal([Id])");
			});

			modelBuilder.Entity<Order>().HasQueryFilter(x => x.CustomerId == CustomerId);


			modelBuilder.Entity<OrderDetail>(entity => {

				entity.Property(e => e.UnitCost).HasColumnType("money");
				// esta es una columna calculada, no se actualiza el valor en la base
				// cuando se agrega una columna de este tipo no se puede cambiar se debe eliminar y volver a agregar
				entity.Property(e => e.LineItemTotal).HasColumnType("money").HasComputedColumnSql("[Quantity]*[UnitCost]");
			});

			modelBuilder.Entity<Product>(entity => {
				entity.Property(e => e.UnitCost).HasColumnType("money");
				entity.Property(e => e.CurrentPrice).HasColumnType("money");

				entity.OwnsOne(o => o.Details, pd => {
					pd.Property(p => p.Description).HasColumnName(nameof(ProductDetails.Description));
                    pd.Property(p => p.ModelName).HasColumnName(nameof(ProductDetails. ModelName));
                    pd.Property(p => p.ModelNumber).HasColumnName(nameof(ProductDetails.ModelNumber));
                    pd.Property(p => p.ProductImage).HasColumnName(nameof(ProductDetails.ProductImage));
                    pd.Property(p => p.ProductImageLarge).HasColumnName(nameof(ProductDetails.ProductImageLarge));
                    pd.Property(p => p.ProductImageThumb).HasColumnName(nameof(ProductDetails.ProductImageThumb));


                });

			});

			modelBuilder.Entity<ShoppingCartRecord>(entity => {
				entity.Property(e => e.DateCreated).HasColumnType("datetime").HasDefaultValueSql("getdate()");
				entity.Property(e => e.Quantity).HasDefaultValue(1);
				// HasName esta obsoleto , se usa HasDatabasename
				entity.HasIndex(e=> new {ShoppingCartRecordId = e.Id, e.ProductId, e.CustomerId}).HasDatabaseName("IX_ShoppingCart").IsUnique();
			});

			// global query filter by customerId
			modelBuilder.Entity<ShoppingCartRecord>().HasQueryFilter(x => x.CustomerId == CustomerId);

			modelBuilder.Entity<CartRecordWithProductInfo>().HasNoKey();
            modelBuilder.Entity<OrderDetailWithProductInfo>().HasNoKey();
        }
    }
}

