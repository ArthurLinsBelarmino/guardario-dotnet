using System.ComponentModel.DataAnnotations;
using GuardaRio.Abrigos.Models;

namespace GuardaRio.Abrigos.Dtos;

public class NecessidadeRequest
{
    [Required(ErrorMessage = "O item é obrigatório (ex.: água, colchão, fralda).")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "O item deve ter entre 2 e 80 caracteres.")]
    public string Item { get; set; } = string.Empty;

    [Range(1, 1_000_000, ErrorMessage = "A quantidade necessária deve ser de pelo menos 1.")]
    public int QuantidadeNecessaria { get; set; }

    [Range(0, 1_000_000, ErrorMessage = "A quantidade recebida não pode ser negativa.")]
    public int QuantidadeRecebida { get; set; }

    public bool Urgente { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Informe o abrigo (abrigoId).")]
    public int AbrigoId { get; set; }
}

public record NecessidadeResponse(
    int Id,
    string Item,
    int QuantidadeNecessaria,
    int QuantidadeRecebida,
    int QuantidadeFaltante,
    bool Urgente,
    int AbrigoId,
    string? AbrigoNome)
{
    public static NecessidadeResponse From(NecessidadeAbrigo n) =>
        new(n.Id, n.Item, n.QuantidadeNecessaria, n.QuantidadeRecebida,
            Math.Max(0, n.QuantidadeNecessaria - n.QuantidadeRecebida),
            n.Urgente, n.AbrigoId, n.Abrigo?.Nome);
}
