using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using System;
using Microsoft.AspNetCore.Authorization;

namespace Shop.Controllers
{
    [Route("products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get([FromServices] DataContext context)
        {
            try
            {
                var products = await context
               .Products
               .Include(x => x.Category)
               .AsNoTracking()
               .ToListAsync();

                return Ok(products);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel selecionar a consulta de produtos no banco" });
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetById(
            int id,
            [FromServices] DataContext context
        )
        {
            try
            {
                var product = await context.Products
                    .Include(x => x.Category)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

                return Ok(product);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel encontrar o produto na base de dados" });
            }
        }

        [HttpGet]
        [Route("categories/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategories(
            int id,
            [FromServices] DataContext context
        )
        {
            try
            {
                var products = await context
                    .Products
                    .Include(x => x.Category)
                    .AsNoTracking()
                    .Where(x => x.CategoryId == id)
                    .ToListAsync();

                return Ok(products);
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possivel encontrar os produtos com esta categoria" });
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> Post(
            [FromServices] DataContext context,
            [FromBody] Product model
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Products.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel atualizar o produto no banco de dados" });
            }

        }
    }
}
