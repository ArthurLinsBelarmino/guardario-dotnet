using System.ComponentModel.DataAnnotations;
using GuardaRio.Abrigos.Models;

namespace GuardaRio.Abrigos.Dtos;

public class AbrigoRequest : IValidatableObject
{
    [Required(ErrorMessage = "O nome do abrigo é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O endereço é obrigatório.")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "O endereço deve ter entre 5 e 200 caracteres.")]
    public string Endereco { get; set; } = string.Empty;

    [Range(1, 100_000, ErrorMessage = "A capacidade deve estar entre 1 e 100.000 pessoas.")]
    public int Capacidade { get; set; }

    [Range(0, 100_000, ErrorMessage = "A ocupação não pode ser negativa.")]
    public int Ocupacao { get; set; }

    public bool AceitaPets { get; set; }
    public bool AcessivelPcd { get; set; }

    public bool Fechado { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Informe a área de risco (areaRiscoId).")]
    public int AreaRiscoId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Ocupacao > Capacidade)
            yield return new ValidationResult(
                "A ocupação não pode ser maior que a capacidade do abrigo.",
                new[] { nameof(Ocupacao) });

        if (Fechado && Ocupacao > 0)
            yield return new ValidationResult(
                "Um abrigo fechado não pode ter pessoas abrigadas.",
                new[] { nameof(Fechado) });
    }
}

public record AbrigoResponse(
    int Id,
    string Nome,
    string Endereco,
    int Capacidade,
    int Ocupacao,
    int VagasDisponiveis,
    bool AceitaPets,
    bool AcessivelPcd,
    string Status,
    int AreaRiscoId,
    string? AreaNome)
{
    public static AbrigoResponse From(Abrigo a) =>
        new(a.Id, a.Nome, a.Endereco, a.Capacidade, a.Ocupacao, a.VagasDisponiveis,
            a.AceitaPets, a.AcessivelPcd, a.Status.ToString(), a.AreaRiscoId, a.AreaRisco?.Nome);
}
