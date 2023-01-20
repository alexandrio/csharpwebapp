using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SpyStore.DAL.EfStructures;
using SpyStore.DAL.Repos.Base;
using SpyStore.DAL.Repos.Interfaces;
using SpyStore.Models.Entities;

namespace SpyStore.DAL.Repos
{
	public class CustomerRepo: RepoBase<Customer>, ICustomerRepo
	{
		public CustomerRepo(StoreContext context):base(context)
		{
		}

		internal CustomerRepo(DbContextOptions<StoreContext> options) : base(options)
		{

		}

		public override IEnumerable<Customer> GetAll() => base.GetAll(x => x.FullName);
	}
}

