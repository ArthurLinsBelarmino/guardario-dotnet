using GuardaRio.Abrigos.Data;
using GuardaRio.Abrigos.Dtos;
using GuardaRio.Abrigos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuardaRio.Abrigos.Controllers;

[ApiController]
[Route("api/abrigos")]
[Produces("application/json")]
public class AbrigosController : ControllerBase
{
    private readonly GuardaRioContext _context;

    public AbrigosController(GuardaRioContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AbrigoResponse>>> Listar(
        [FromQuery] int? areaId,
        [FromQuery] bool? aceitaPets,
        [FromQuery] bool? acessivelPcd,
        [FromQuery] bool? somenteComVaga)
    {
        var query = _context.Abrigos.AsNoTracking().Include(a => a.AreaRisco).AsQueryable();

        if (areaId.HasValue) query = query.Where(a => a.AreaRiscoId == areaId.Value);
        if (aceitaPets.HasValue) query = query.Where(a => a.AceitaPets == aceitaPets.Value);
        if (acessivelPcd.HasValue) query = query.Where(a => a.AcessivelPcd == acessivelPcd.Value);
        if (somenteComVaga == true) query = query.Where(a => a.Status == StatusAbrigo.ABERTO);

        var abrigos = await query.OrderBy(a => a.Nome).ToListAsync();
        return Ok(abrigos.Select(AbrigoResponse.From));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AbrigoResponse>> BuscarPorId(int id)
    {
        var abrigo = await _context.Abrigos
            .AsNoTracking()
            .Include(a => a.AreaRisco)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (abrigo is null)
            return NotFound(new ErroResponse($"Abrigo {id} não encontrado."));

        return Ok(AbrigoResponse.From(abrigo));
    }

    [HttpGet("{id:int}/necessidades")]
    public async Task<ActionResult<IEnumerable<NecessidadeResponse>>> ListarNecessidades(int id)
    {
        var abrigo = await _context.Abrigos
            .AsNoTracking()
            .Include(a => a.Necessidades)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (abrigo is null)
            return NotFound(new ErroResponse($"Abrigo {id} não encontrado."));

        var lista = abrigo.Necessidades
            .OrderByDescending(n => n.Urgente)
            .ThenBy(n => n.Item)
            .Select(n => new NecessidadeResponse(
                n.Id, n.Item, n.QuantidadeNecessaria, n.QuantidadeRecebida,
                Math.Max(0, n.QuantidadeNecessaria - n.QuantidadeRecebida),
                n.Urgente, n.AbrigoId, abrigo.Nome));

        return Ok(lista);
    }

    [HttpPost]
    public async Task<ActionResult<AbrigoResponse>> Criar(AbrigoRequest request)
    {
        var area = await _context.Areas.FindAsync(request.AreaRiscoId);
        if (area is null)
            return BadRequest(new ErroResponse($"Área de risco {request.AreaRiscoId} não existe."));

        var abrigo = new Abrigo { AreaRisco = area };
        Preencher(abrigo, request);

        _context.Abrigos.Add(abrigo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(BuscarPorId), new { id = abrigo.Id }, AbrigoResponse.From(abrigo));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, AbrigoRequest request)
    {
        var abrigo = await _context.Abrigos.FindAsync(id);
        if (abrigo is null)
            return NotFound(new ErroResponse($"Abrigo {id} não encontrado."));

        var areaExiste = await _context.Areas.AnyAsync(a => a.Id == request.AreaRiscoId);
        if (!areaExiste)
            return BadRequest(new ErroResponse($"Área de risco {request.AreaRiscoId} não existe."));

        Preencher(abrigo, request);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var abrigo = await _context.Abrigos.FindAsync(id);
        if (abrigo is null)
            return NotFound(new ErroResponse($"Abrigo {id} não encontrado."));

        _context.Abrigos.Remove(abrigo);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static void Preencher(Abrigo abrigo, AbrigoRequest request)
    {
        abrigo.Nome = request.Nome.Trim();
        abrigo.Endereco = request.Endereco.Trim();
        abrigo.Capacidade = request.Capacidade;
        abrigo.Ocupacao = request.Ocupacao;
        abrigo.AceitaPets = request.AceitaPets;
        abrigo.AcessivelPcd = request.AcessivelPcd;
        abrigo.AreaRiscoId = request.AreaRiscoId;
        abrigo.AtualizarStatus(request.Fechado);
    }
}
