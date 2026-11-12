using GuardaRio.Abrigos.Models;
using Microsoft.EntityFrameworkCore;

namespace GuardaRio.Abrigos.Data;

public class GuardaRioContext : DbContext
{
    public GuardaRioContext(DbContextOptions<GuardaRioContext> options) : base(options) { }

    public DbSet<AreaRisco> Areas => Set<AreaRisco>();
    public DbSet<Abrigo> Abrigos => Set<Abrigo>();
    public DbSet<NecessidadeAbrigo> Necessidades => Set<NecessidadeAbrigo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AreaRisco>(e =>
        {
            e.ToTable("TB_GR_AREA");
            e.HasKey(a => a.Id).HasName("PK_GR_AREA");
            e.Property(a => a.Id).HasColumnName("ID");
            e.Property(a => a.Nome).HasColumnName("NOME").HasMaxLength(100).IsRequired();
            e.Property(a => a.Cidade).HasColumnName("CIDADE").HasMaxLength(80).IsRequired();
            e.Property(a => a.Uf).HasColumnName("UF").HasMaxLength(2).IsRequired();
            e.Property(a => a.PopulacaoEstimada).HasColumnName("POPULACAO_ESTIMADA");
        });

        modelBuilder.Entity<Abrigo>(e =>
        {
            e.ToTable("TB_GR_ABRIGO", t =>
            {
                t.HasCheckConstraint("CK_GR_ABRIGO_OCUPACAO", "OCUPACAO BETWEEN 0 AND CAPACIDADE");
            });
            e.HasKey(a => a.Id).HasName("PK_GR_ABRIGO");
            e.Property(a => a.Id).HasColumnName("ID");
            e.Property(a => a.Nome).HasColumnName("NOME").HasMaxLength(100).IsRequired();
            e.Property(a => a.Endereco).HasColumnName("ENDERECO").HasMaxLength(200).IsRequired();
            e.Property(a => a.Capacidade).HasColumnName("CAPACIDADE");
            e.Property(a => a.Ocupacao).HasColumnName("OCUPACAO");
            e.Property(a => a.AceitaPets).HasColumnName("ACEITA_PETS");
            e.Property(a => a.AcessivelPcd).HasColumnName("ACESSIVEL_PCD");
            e.Property(a => a.Status).HasColumnName("STATUS").HasConversion<string>().HasMaxLength(10);
            e.Property(a => a.AreaRiscoId).HasColumnName("AREA_ID");
            e.Ignore(a => a.VagasDisponiveis);

            e.HasOne(a => a.AreaRisco)
             .WithMany(ar => ar.Abrigos)
             .HasForeignKey(a => a.AreaRiscoId)
             .HasConstraintName("FK_GR_ABRIGO_AREA")
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<NecessidadeAbrigo>(e =>
        {
            e.ToTable("TB_GR_NECESSIDADE");
            e.HasKey(n => n.Id).HasName("PK_GR_NECESSIDADE");
            e.Property(n => n.Id).HasColumnName("ID");
            e.Property(n => n.Item).HasColumnName("ITEM").HasMaxLength(80).IsRequired();
            e.Property(n => n.QuantidadeNecessaria).HasColumnName("QTD_NECESSARIA");
            e.Property(n => n.QuantidadeRecebida).HasColumnName("QTD_RECEBIDA");
            e.Property(n => n.Urgente).HasColumnName("URGENTE");
            e.Property(n => n.AbrigoId).HasColumnName("ABRIGO_ID");

            e.HasOne(n => n.Abrigo)
             .WithMany(a => a.Necessidades)
             .HasForeignKey(n => n.AbrigoId)
             .HasConstraintName("FK_GR_NECESSIDADE_ABRIGO")
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
