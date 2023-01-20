using System;
using System.Collections.Generic;
using System.Data;

using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;

using Microsoft.EntityFrameworkCore;
using SpyStore.DAL.EfStructures;
using SpyStore.DAL.Repos.Base;
using SpyStore.DAL.Repos.Interfaces;
using SpyStore.DAL.Exceptions;
using SpyStore.Models.Entities;
using SpyStore.Models.ViewModels;

namespace SpyStore.DAL.Repos
{
	public class ShoppingCartRepo : RepoBase<ShoppingCartRecord>, IShoppingCartRepo
	{
        private readonly IProductRepo _productRepo;

        private readonly ICustomerRepo _customerRepo;




		public ShoppingCartRepo(StoreContext context,IProductRepo productRepo, ICustomerRepo customerRepo):base(context)
		{
            _productRepo = productRepo;
            _customerRepo = customerRepo;
		}


        internal ShoppingCartRepo(DbContextOptions<StoreContext> options, IProductRepo productRepo, ICustomerRepo customerRepo) :base(new StoreContext(options))
        {
            _productRepo = productRepo;
            _customerRepo = customerRepo;
            base.Dispose();
        }

        public override void Dispose()
        {
            _productRepo.Dispose();
            _customerRepo.Dispose();
            base.Dispose();
        }


        public override IEnumerable<ShoppingCartRecord> GetAll() => base.GetAll(x => x.DateCreated).ToList();


        public ShoppingCartRecord GetBy(int productId) => Table.FirstOrDefault(x => x.ProductId == productId);



        public CartRecordWithProductInfo GetShoppingCartRecord(int id) => Context.CartRecordWithProductInfos.FirstOrDefault(x => x.Id == id);


        public IEnumerable<CartRecordWithProductInfo> GetShoppingCartRecords(int customerId) => Context.CartRecordWithProductInfos
                                                                                                        .Where(x => x.CustomerId == customerId)
                                                                                                        .OrderBy(x => x.ModelName);


        public CartWIthCustomerInfo GetShoppingCartRecordsWithCustomer(int customerId) =>
            new CartWIthCustomerInfo()
            {
                CartRecords = GetShoppingCartRecords(customerId).ToList(),
                Customer = _customerRepo.Find(customerId)
            };


        public int Purchase(int customerId)
        {
            var customerIdParam = new SqlParameter("@customerId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Input,
                Value = customerId
            };

            var orderIdParam = new SqlParameter("@orderId", SqlDbType.Int)
            {
                Direction  =ParameterDirection.Output
            };

            try
            {
                Context.Database.ExecuteSqlRaw("EXEC [Store].[PurchaseItemsInCart] @customerId, @orderid out", new { customerIdParam, orderIdParam });

                //Context.Database.ExecuteSql("EXEC [Store].[PurchaseItemsInCart] @customerId, @orderid out", customerIdParam, orderIdParam);
            }catch(Exception ex)
            {
                return -1;
            }
            return (int)orderIdParam.Value;
        }

        public override int Add(ShoppingCartRecord entity,  bool persist = true)
        {
            var product = _productRepo.FindAsNoTracking(entity.ProductId);
            if(product== null)
            {
                throw new SpyStoreInvalidProductException("no hay de este");
            }
            return Add(entity, product, persist);
        }


        public int Add(ShoppingCartRecord entity, Product product, bool persist = true)
        {
            var item = GetBy(entity.ProductId);
            if (item == null)
            {
                if(entity.Quantity > product.UnitsInStock)
                {
                    throw new SpyStoreInvalidQuantityException("Como te doy si no hay");
                }
                entity.LineItemTotal = entity.Quantity * product.CurrentPrice;
                return base.Add(entity, persist);
            }

            item.Quantity += entity.Quantity;
            return item.Quantity <= 0 ? Delete(item, persist) : Update(item, product, persist);
            
        }

        public override int AddRange(IEnumerable<ShoppingCartRecord> entities, bool persist = true)
        {
            int counter = 0;
            foreach (var item in entities)
            {
                var product = _productRepo.FindAsNoTracking(item.ProductId);
                counter += Add(item, product, false);
            }
            return persist ? SaveChanges() : counter;
        }

        public override int UpdateRange(IEnumerable<ShoppingCartRecord> entities, bool persist = true)
        {
            int counter = 0;
            foreach(var item in entities)
            {
                var product = _productRepo.FindAsNoTracking(item.ProductId);
                counter += Update(item, product, false);
                
            }
            return persist ? SaveChanges() : counter;
        }

        public override int Update(ShoppingCartRecord entity, bool persist = true)
        {
            var product = _productRepo.FindAsNoTracking(entity.ProductId);
            if(product == null)
            {
                throw new SpyStoreInvalidProductException("No se encuentra el producto");
            }
            return Update(entity, product, persist);
        }

        public int Update(ShoppingCartRecord entity, Product product, bool persist = true)
        {
            if(entity.Quantity <= 0)
            {
                return Delete(entity, persist);
            }

            if(entity.Quantity > product.UnitsInStock)
            {
                throw new SpyStoreInvalidQuantityException("No se puede vender lo que no se tiene");
            }

            var dbRecord = Find(entity.Id);
            if(entity.TimeStamp != null && dbRecord.TimeStamp.SequenceEqual(entity.TimeStamp))
            {
                dbRecord.Quantity = entity.Quantity;
                dbRecord.LineItemTotal = entity.Quantity * product.CurrentPrice;
                return base.Update(dbRecord, persist);

            }
            throw new SpyStoreConcurrencyException("Alquien le cambio antes de moverle");
        }
    }
}

