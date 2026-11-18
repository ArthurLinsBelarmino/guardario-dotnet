using GuardaRio.Abrigos.Data;
using GuardaRio.Abrigos.Dtos;
using GuardaRio.Abrigos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuardaRio.Abrigos.Controllers;

[ApiController]
[Route("api/areas")]
[Produces("application/json")]
public class AreasController : ControllerBase
{
    private readonly GuardaRioContext _context;

    public AreasController(GuardaRioContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AreaResponse>>> Listar()
    {
        var areas = await _context.Areas
            .AsNoTracking()
            .Include(a => a.Abrigos)
            .OrderBy(a => a.Nome)
            .ToListAsync();

        return Ok(areas.Select(AreaResponse.From));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AreaResponse>> BuscarPorId(int id)
    {
        var area = await _context.Areas
            .AsNoTracking()
            .Include(a => a.Abrigos)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (area is null)
            return NotFound(new ErroResponse($"Área de risco {id} não encontrada."));

        return Ok(AreaResponse.From(area));
    }

    [HttpPost]
    public async Task<ActionResult<AreaResponse>> Criar(AreaRequest request)
    {
        var area = new AreaRisco
        {
            Nome = request.Nome.Trim(),
            Cidade = request.Cidade.Trim(),
            Uf = request.Uf.ToUpperInvariant(),
            PopulacaoEstimada = request.PopulacaoEstimada
        };

        _context.Areas.Add(area);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(BuscarPorId), new { id = area.Id }, AreaResponse.From(area));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, AreaRequest request)
    {
        var area = await _context.Areas.FindAsync(id);
        if (area is null)
            return NotFound(new ErroResponse($"Área de risco {id} não encontrada."));

        area.Nome = request.Nome.Trim();
        area.Cidade = request.Cidade.Trim();
        area.Uf = request.Uf.ToUpperInvariant();
        area.PopulacaoEstimada = request.PopulacaoEstimada;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var area = await _context.Areas.FindAsync(id);
        if (area is null)
            return NotFound(new ErroResponse($"Área de risco {id} não encontrada."));

        var temAbrigos = await _context.Abrigos.CountAsync(a => a.AreaRiscoId == id) > 0;
        if (temAbrigos)
            return Conflict(new ErroResponse(
                "Não é possível excluir a área: ainda existem abrigos vinculados a ela."));

        _context.Areas.Remove(area);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

