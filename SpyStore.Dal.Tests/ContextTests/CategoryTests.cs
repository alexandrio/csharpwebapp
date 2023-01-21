using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using SpyStore.DAL.EfStructures;
using SpyStore.DAL.Inicialization;
using SpyStore.Models.Entities;
using Xunit;

namespace SpyStore.Dal.Tests.ContextTests
{

    //All classes with the Collection attribute and the same key have   their tests run in serial.
    [Collection("SpyStore.DAL")]
    public class CategoryTests: IDisposable
    {
        private readonly StoreContext _db;


        public CategoryTests()
        {
            _db = new StoreContextFactory().CreateDbContext( new string[0]);
            CleanDatabase();
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        private void CleanDatabase()
        {
            SampleDataInitializer.ClearData(_db);
        }


        [Fact]
        public void FirstTest()
        {
            Assert.True(true);
        }

        [Fact]
        public void ShouldAddACategoryWithDbSet()
        {
            var category = new Category { CategoryName = "Foo" };
            _db.Categories.Add(category);
            Assert.Equal(EntityState.Added, _db.Entry(category).State);
            int value = int.Parse(_db.Entry(category).Property("Id").CurrentValue.ToString());
            Assert.True( value < 0);
            Assert.Null(category.TimeStamp);
            _db.SaveChanges();
            Assert.Equal(EntityState.Unchanged, _db.Entry(category).State);
            Assert.NotNull(category.TimeStamp);
            Assert.Equal(1, _db.Categories.Count());
        }
        [Fact]
        public void ShouldAddACategoryWithContext()
        {
            var category = new Category { CategoryName = "Foo" };
            _db.Add(category);
            Assert.Equal(EntityState.Added, _db.Entry(category).State);
            int value = int.Parse(_db.Entry(category).Property("Id").CurrentValue.ToString());
            Assert.True(value <0);
            Assert.Null(category.TimeStamp);
            _db.SaveChanges();
            Assert.Equal(EntityState.Unchanged, _db.Entry(category).State);
            Assert.NotNull(category.TimeStamp);
            Assert.Equal(1, _db.Categories.Count());
        }

        [Fact]
        public void ShouldGetAllCategoriesOrderedByName()
        {
            _db.Categories.Add(new Category { CategoryName = "Foo" });
            _db.Categories.Add(new Category { CategoryName = "Bar" });
            _db.Categories.Add(new Category { CategoryName = "New" });
            _db.Categories.Add(new Category { CategoryName = "End" });
            _db.SaveChanges();
            var categories = _db.Categories.OrderBy(c => c.CategoryName).ToList();
            Assert.Equal(4, _db.Categories.Count());
            Assert.Equal("Foo", categories[2].CategoryName);
            Assert.Equal("New", categories[3].CategoryName);
        }

        [Fact]
        public void ShouldUpdateACategory()
        {
            var category = new Category { CategoryName= "Foo" };
            _db.Categories.Add(category);
            _db.SaveChanges();
            category.CategoryName = "Bar";
            _db.Categories.Update(category);
            Assert.Equal(EntityState.Modified, _db.Entry(category).State);
            _db.SaveChanges();
            Assert.Equal(EntityState.Unchanged, _db.Entry(category).State);
            StoreContext context;
            using(context= new StoreContextFactory().CreateDbContext(null))
            {
                Assert.Equal("Bar", context.Categories.First().CategoryName);
            }
        }

        [Fact]
        public void ShouldNotUpdateANonAttachedCategory()
        {
            var category = new Category { CategoryName = "Foo" };
            _db.Categories.Add(category);
            category.CategoryName = "Bar";
            Assert.Throws<InvalidOperationException>(() => _db.Categories.Update(category));
        }

        [Fact]
        public void ShouldDeleteACategory()
        {
            var category = new Category { CategoryName = "Foo" };
            _db.Categories.Add(category);
            _db.SaveChanges();
            Assert.Equal(1, _db.Categories.Count());
            _db.Categories.Remove(category);
            Assert.Equal(EntityState.Deleted, _db.Entry(category).State);
            _db.SaveChanges();
            Assert.Equal(EntityState.Detached, _db.Entry(category).State);
            Assert.Equal(0, _db.Categories.Count());
        }

        [Fact]
        public void ShouldDeleteACategoryWithTimestampData()
        {
            var category = new Category { CategoryName = "Foo" };
            _db.Categories.Add(category);
            _db.SaveChanges();
            var context = new StoreContextFactory().CreateDbContext(null);
            var catToDelete = new Category { Id = category.Id, TimeStamp = category.TimeStamp };
            context.Entry(catToDelete).State = EntityState.Deleted;
        }

        [Fact]
        public void ShouldNotDeleteACategoryWithoutTimestampData()
        {
            var category = new Category { CategoryName = "Foo" };
            _db.Categories.Add(category);
            _db.SaveChanges();
            var context = new StoreContextFactory().CreateDbContext(null);
            var catToDelete = new Category { Id = category.Id };
            context.Categories.Remove(catToDelete);
            var ex = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());
            Assert.Equal(1, ex.Entries.Count);
            Assert.Equal(category.Id, ((Category)ex.Entries[0].Entity).Id);
        }

    }
}
