using System;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SpyStore.DAL.EfStructures;
using SpyStore.DAL.Exceptions;
using SpyStore.Models.Entities.Base;

namespace SpyStore.DAL.Repos.Base
{

	// constrain the type to entitybase and new()
	public abstract class RepoBase<T> :IRepo<T> where T: EntityBase, new()
	{
        

        public DbSet<T> Table { get; }
        public StoreContext Context { get; }

        public (string Schema, string Tablename) TableSchemaAndName
        {
            get
            {
                // se reemplaza por getschema y gettablename
                //var metaData = Context.Model.FindEntityType(typeof(T).FullName.SqlServer();
                
                string? clase = typeof(T).FullName;
                var metaData = Context.Model.FindEntityType(clase);
                string? schema = metaData.GetSchema();
                string? tableName = metaData.GetTableName();
                return (schema, tableName);
            }
        }

        public bool HasChanges => Context.ChangeTracker.HasChanges();

        private readonly bool _disposeContext;


        protected RepoBase(DbContextOptions<StoreContext> options) :this (new StoreContext(options))
		{
			_disposeContext = true;
		}

		public virtual void Dispose()
		{
			if (_disposeContext)
			{
				Context.Dispose();
			}
		}

        public T Find(int? id)
        {
            return Table.Find(id);
        }

        public T FindAsNoTracking(int id) => Table.Where(x => x.Id == id).AsNoTracking().FirstOrDefault();


        public T FindIgnoreQueryFilters(int id) => Table.IgnoreQueryFilters().FirstOrDefault(x => x.Id == id);


        public virtual IEnumerable<T> GetAll() => Table;


        public virtual IEnumerable<T> GetAll(Expression<Func<T, object>> orderBy) => Table.OrderBy(orderBy);


        public IEnumerable<T> GetRange(IQueryable<T> query, int skip, int take) => query.Skip(skip).Take(take);


        public virtual int Add(T entity, bool persist = true)
        {
            Table.Add(entity);
            return persist ? SaveChanges() : 0;
        }

        public virtual int AddRange(IEnumerable<T> entities, bool persist = true)
        {
            Table.AddRange(entities);
            return persist ? SaveChanges() : 0; 
        }

        public virtual int Update(T entity, bool persist = true)
        {
            Table.Update(entity);
            return persist ? SaveChanges() : 0;
        }

        public virtual int UpdateRange(IEnumerable<T> entities, bool persist = true)
        {
            Table.UpdateRange(entities);
            return persist ? SaveChanges() : 0;
        }

        public virtual int Delete(T entity, bool persist = true)
        {
            Table.Remove(entity);
            return persist ? SaveChanges() : 0;
        }

        public virtual int DeleteRange(IEnumerable<T> entities, bool persist = true)
        {
            Table.RemoveRange(entities);
            return persist ? SaveChanges() : 0;
        }

        public int SaveChanges()
        {
            try
            {
                return Context.SaveChanges();
            }catch(DbUpdateConcurrencyException ex)
            {
                throw new SpyStoreConcurrencyException("A concurrency error happened.", ex);
            }
            catch (RetryLimitExceededException ex)
            {
                throw new SpyStoreRetryLimitedExceededException("Retry limited exceeded, connection problem.", ex);
            }catch(DbUpdateException ex)
            {
                if(ex.InnerException is SqlException sqlexception)
                {
                    if(sqlexception.Message.Contains("FOREIGN KEY constraint", StringComparison.OrdinalIgnoreCase))
                    {
                        if (sqlexception.Message.Contains("table \"Store.Products\", column 'Id'", StringComparison.OrdinalIgnoreCase))
                        {
                            throw new SpyStoreInvalidProductException($"Invalid Product Id\r\n{ex.Message}", ex);
                        }

                        if (sqlexception.Message.Contains("table \"Store.Customers\", column 'Id'", StringComparison.OrdinalIgnoreCase))
                        {
                            throw new SpyStoreInvalidCustomerException($"Invalid Customer Id\r\n{ex.Message}", ex);
                        }
                    }
                }

                throw new SpyStoreException("An error ocurred updating the database", ex);
            }catch(Exception ex)
            {
                throw new SpyStoreException("An error ocurred updating the database", ex);
            }
        }

        protected RepoBase(StoreContext context)
		{
			Context = context;
			Table = Context.Set<T>();
		}
	}
}

