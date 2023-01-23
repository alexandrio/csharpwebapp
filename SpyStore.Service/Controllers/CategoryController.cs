using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SpyStore.DAL.Repos;
using SpyStore.DAL.Repos.Interfaces;
using SpyStore.Models.Entities;

namespace SpyStore.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepo _repo;
        public CategoryController(ICategoryRepo repo)
        {
            _repo = repo;
        }

        [HttpGet(Name ="GetAllCategories")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public ActionResult<IList<Category>> Get()
        {
            IEnumerable<Category> categories = _repo.GetAll().ToList();
            return Ok(categories);
        }


        [HttpGet("{id}",Name = "GetCategory")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult<IList<Category>> Get(int id)
        {
            Category item = _repo.Find(id);
            if(item == null){
                return NotFound();
            }
            return Ok(item);
        }

        [HttpGet("{categoryId}/products", Name = "GetCategoryProducts")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public ActionResult<IList<Product>> GetProductsForCategory(
            [FromServices] IProductRepo productRepo, int categoryId
            ) => productRepo.GetProductsForCategory(categoryId).ToList();



    }
}

