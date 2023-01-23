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
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepo _repo;
        public CustomerController(ICustomerRepo repo)
        {
            _repo = repo;
        }

        [HttpGet(Name ="GetAllCustomers")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public ActionResult<IEnumerable<Customer>> Get() => Ok(_repo.GetAll().ToList());

        [HttpGet("{id}",Name ="GetCustomer")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult<Customer> Get(int id)
        {
            var item = _repo.Find(id);
            if(item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

    }
}

