namespace GuardaRio.Abrigos.Models;

public class Abrigo
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public int Capacidade { get; set; }
    public int Ocupacao { get; set; }
    public bool AceitaPets { get; set; }
    public bool AcessivelPcd { get; set; }
    public StatusAbrigo Status { get; set; } = StatusAbrigo.ABERTO;

    public int AreaRiscoId { get; set; }
    public AreaRisco? AreaRisco { get; set; }

    public List<NecessidadeAbrigo> Necessidades { get; set; } = new();

    public int VagasDisponiveis => Math.Max(0, Capacidade - Ocupacao);

    // RN05: o status é calculado, nunca informado direto.
    // FECHADO só por decisão do agente; senão LOTADO quando chega na capacidade.
    public void AtualizarStatus(bool fechado)
    {
        if (fechado)
            Status = StatusAbrigo.FECHADO;
        else
            Status = Ocupacao >= Capacidade ? StatusAbrigo.LOTADO : StatusAbrigo.ABERTO;
    }
}
