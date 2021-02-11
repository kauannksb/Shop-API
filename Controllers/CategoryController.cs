using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

//endpoint = URL
//http://localhost:5000
//https://localhost:5001
namespace Shop.Controllers
{
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Category>>> Get(
            [FromServices] DataContext context
        )
        {
            var categories = await context.Categories.AsNoTracking().ToListAsync();
            return Ok(categories);
        }

        [HttpGet]
        [Route("{id:int}")]
        //para fazer restricao de rota, basta colocar ao lado do parametro passo o tipo, como no exemplo acima
        //nessa rota, por exemplo, o id so pode ser do tipo int.
        //caso seja passado outro tipo, retorna 404 na rota 
        //esse parametro é capturado da url
        [AllowAnonymous]
        public async Task<ActionResult<List<Category>>> GetById(
            int id,
            [FromServices] DataContext context
         )
        {
            try
            {
                var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                return Ok(category);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel retornar os registros do banco de dados" });
            }

        }

        [HttpPost]
        [Route("")]
        //o FromBody é para o back saber que o parametro que esta sendo passado, vem do corpo do json e não da url
        //o ModelState é pra validar se o que estamos recebendo no corpo do json esta atendendo todas as premissas criadas no modelo. 
        //Exemplo: Se o tamanho da string da categoria é maior que 3
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Category>>> Post(
            [FromBody] Category model,
            [FromServices] DataContext context
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Categories.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possivel criar uma nova categoria" });
            }

        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Category>>> Put(
            int id,
            [FromBody] Category model,
            [FromServices] DataContext context
            )
        {
            //Verifica se o ID informado é o mesmo do modelo
            if (id != model.Id)
                return NotFound(new { message = "Categoria não encontrada" + ". id: " + id + ", model.Id: " + model.Id });

            //Verifica se os dados estão validos
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);

            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Não foi possivel atualizar a categoria. Possivel problema de concorrencia de dados" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel atualizar a categoria" });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Category>>> Delete(
            int id,
            [FromServices] DataContext context
        )
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
            {
                return NotFound(new { message = "Categoria não encontrada" });
            }

            try
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok(new { message = "Categoria deletado com sucesso!" });
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possivel remover a categoria" });
            }
        }
    }
}
