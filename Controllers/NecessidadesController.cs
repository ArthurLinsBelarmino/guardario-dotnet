using GuardaRio.Abrigos.Data;
using GuardaRio.Abrigos.Dtos;
using GuardaRio.Abrigos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuardaRio.Abrigos.Controllers;

[ApiController]
[Route("api/necessidades")]
[Produces("application/json")]
public class NecessidadesController : ControllerBase
{
    private readonly GuardaRioContext _context;

    public NecessidadesController(GuardaRioContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NecessidadeResponse>>> Listar(
        [FromQuery] int? abrigoId,
        [FromQuery] bool? urgente)
    {
        var query = _context.Necessidades.AsNoTracking().Include(n => n.Abrigo).AsQueryable();

        if (abrigoId.HasValue) query = query.Where(n => n.AbrigoId == abrigoId.Value);
        if (urgente.HasValue) query = query.Where(n => n.Urgente == urgente.Value);

        var lista = await query
            .OrderByDescending(n => n.Urgente)
            .ThenBy(n => n.Item)
            .ToListAsync();

        return Ok(lista.Select(NecessidadeResponse.From));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<NecessidadeResponse>> BuscarPorId(int id)
    {
        var necessidade = await _context.Necessidades
            .AsNoTracking()
            .Include(n => n.Abrigo)
            .FirstOrDefaultAsync(n => n.Id == id);

        if (necessidade is null)
            return NotFound(new ErroResponse($"Necessidade {id} não encontrada."));

        return Ok(NecessidadeResponse.From(necessidade));
    }

    [HttpPost]
    public async Task<ActionResult<NecessidadeResponse>> Criar(NecessidadeRequest request)
    {
        var abrigo = await _context.Abrigos.FindAsync(request.AbrigoId);
        if (abrigo is null)
            return BadRequest(new ErroResponse($"Abrigo {request.AbrigoId} não existe."));

        var necessidade = new NecessidadeAbrigo { Abrigo = abrigo };
        Preencher(necessidade, request);

        _context.Necessidades.Add(necessidade);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(BuscarPorId), new { id = necessidade.Id },
            NecessidadeResponse.From(necessidade));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, NecessidadeRequest request)
    {
        var necessidade = await _context.Necessidades.FindAsync(id);
        if (necessidade is null)
            return NotFound(new ErroResponse($"Necessidade {id} não encontrada."));

        var abrigoExiste = await _context.Abrigos.AnyAsync(a => a.Id == request.AbrigoId);
        if (!abrigoExiste)
            return BadRequest(new ErroResponse($"Abrigo {request.AbrigoId} não existe."));

        Preencher(necessidade, request);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var necessidade = await _context.Necessidades.FindAsync(id);
        if (necessidade is null)
            return NotFound(new ErroResponse($"Necessidade {id} não encontrada."));

        _context.Necessidades.Remove(necessidade);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static void Preencher(NecessidadeAbrigo necessidade, NecessidadeRequest request)
    {
        necessidade.Item = request.Item.Trim();
        necessidade.QuantidadeNecessaria = request.QuantidadeNecessaria;
        necessidade.QuantidadeRecebida = request.QuantidadeRecebida;
        necessidade.Urgente = request.Urgente;
        necessidade.AbrigoId = request.AbrigoId;
    }
}
