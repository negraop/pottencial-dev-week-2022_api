using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

using src.Models;
using src.Persistence;

namespace src.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PessoaController : ControllerBase
{
    private DatabaseContext _context { get; set; }

    public PessoaController(DatabaseContext context)
    {
        this._context = context;
    }


    [HttpGet]
    public async Task<ActionResult<List<Pessoa>>> Get()
    {
        var result = await _context.Pessoas.Include(p => p.Contratos).ToListAsync();

        if (!result.Any())
            return NoContent();
        
        return Ok(result); 
    }


    [HttpPost]
    public async Task<ActionResult<Pessoa>> Post([FromBody] Pessoa pessoa)
    {
        try
        {
            await _context.Pessoas.AddAsync(pessoa);
            await _context.SaveChangesAsync();
        }
        catch (System.Exception e)
        {
            return BadRequest(new {
                status = HttpStatusCode.NotFound,
                msg = $"Erro ao inserir a pessoa: {e}"
            });
        }

        return Created("[api]/pessoa", pessoa);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Object>> Update(
        [FromRoute] int id,
        [FromBody] Pessoa pessoa
    )
    {
        var result = await _context.Pessoas.SingleOrDefaultAsync(e => e.Id == id);

        if (result is null)
        {
            return NotFound(new {
                status = HttpStatusCode.NotFound,
                msg = "Registro não encontrado"
            });
        }
        
        try
        {
            _context.ChangeTracker.Clear();
            _context.Pessoas.Update(pessoa);
            await _context.SaveChangesAsync();
        }
        catch (System.Exception e)
        {
            return BadRequest(new {
                status = HttpStatusCode.BadRequest,
                msg = $"Houve erro para atualizar o registro: {e}"
            });
        }

        return Ok(new {
            status = HttpStatusCode.OK,
            msg = $"Dados do id {id} atualizados"
        });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Object>> Delete([FromRoute] int id)
    {
        var result = await _context.Pessoas.SingleOrDefaultAsync(e => e.Id == id);

        if (result is null)
            return NotFound(new {
                status = HttpStatusCode.NotFound,
                msg = "Registro não encontrado"
            });
        
        _context.Pessoas.Remove(result);
        await _context.SaveChangesAsync();

        return Ok(new {
            status = HttpStatusCode.OK,
            msg = $"Deletado pessoa de Id {id}"
        });
    }
}