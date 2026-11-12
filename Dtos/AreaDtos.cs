using System.ComponentModel.DataAnnotations;
using GuardaRio.Abrigos.Models;

namespace GuardaRio.Abrigos.Dtos;

public class AreaRequest
{
    [Required(ErrorMessage = "O nome da área é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A cidade é obrigatória.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "A cidade deve ter entre 2 e 80 caracteres.")]
    public string Cidade { get; set; } = string.Empty;

    [Required(ErrorMessage = "A UF é obrigatória.")]
    [RegularExpression("^[A-Za-z]{2}$", ErrorMessage = "A UF deve ter 2 letras, ex.: SP.")]
    public string Uf { get; set; } = string.Empty;

    [Range(0, 50_000_000, ErrorMessage = "A população estimada deve estar entre 0 e 50.000.000.")]
    public int PopulacaoEstimada { get; set; }
}

public record AreaResponse(
    int Id,
    string Nome,
    string Cidade,
    string Uf,
    int PopulacaoEstimada,
    int TotalAbrigos)
{
    public static AreaResponse From(AreaRisco a) =>
        new(a.Id, a.Nome, a.Cidade, a.Uf, a.PopulacaoEstimada, a.Abrigos.Count);
}
