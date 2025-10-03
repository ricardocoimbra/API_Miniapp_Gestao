using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using API_Miniapp_Gestao.Constants;

namespace API_Miniapp_Gestao.Models;

[ExcludeFromCodeCoverage]
public class DbEscrita : DbContextBase
{
    public DbEscrita(DbContextOptions<DbEscrita> options) : base(options) { }
}

[ExcludeFromCodeCoverage]
public class DbLeitura : DbContextBase
{
    public DbLeitura(DbContextOptions<DbLeitura> options) : base(options) { }
}

[ExcludeFromCodeCoverage]
public abstract class DbContextBase : DbContext
{
    protected DbContextBase(DbContextOptions options) : base(options)
    {
    }

    public virtual DbSet<Nbmtb001VersaoSistemaOperacional> Nbmtb001VersaoSistemaOperacionals { get; set; }

    public virtual DbSet<Nbmtb002ModeloDevice> Nbmtb002ModeloDevices { get; set; }

    public virtual DbSet<Nbmtb003VersaoAplicativo> Nbmtb003VersaoAplicativos { get; set; }

    public virtual DbSet<Nbmtb004Miniapp> Nbmtb004Miniapps { get; set; }

    public virtual DbSet<Nbmtb005RelacionamentoMiniapp> Nbmtb005RelacionamentoMiniapps { get; set; }

    public virtual DbSet<Nbmtb006VersaoMiniapp> Nbmtb006VersaoMiniapps { get; set; }

    public virtual DbSet<Nbmtb010UsuarioVersaoMiniapp> Nbmtb010UsuarioVersaoMiniapps { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Nbmtb001VersaoSistemaOperacional>(entity =>
        {
            entity.HasKey(e => e.CoVersaoSistemaOperacional);

            entity.ToTable(DatabaseConstants.TableNames.NBMTB001_VERSAO_SISTEMA_OPERACIONAL);

            // Índice único para evitar duplicação de versão por plataforma
            entity.HasIndex(e => new { e.CoPlataforma, e.NuVersaoSistemaOperacional })
                .IsUnique()
                .HasDatabaseName("IX_NBMTB001_PLATAFORMA_VERSAO");

            entity.Property(e => e.CoVersaoSistemaOperacional)
                .ValueGeneratedNever()
                .HasColumnName(DatabaseConstants.ColumnNames.CO_VERSAO_SISTEMA_OPERACIONAL);

            entity.Property(e => e.CoPlataforma)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .IsRequired()
                .HasColumnName(DatabaseConstants.ColumnNames.CO_PLATAFORMA);

            entity.Property(e => e.NuVersaoSistemaOperacional)
                .HasColumnType("decimal(6, 3)")
                .IsRequired()
                .HasColumnName(DatabaseConstants.ColumnNames.NU_VERSAO_SISTEMA_OPERACIONAL);

            // Check constraints para validação de dados
            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_NBMTB001_CO_PLATAFORMA", "[CO_PLATAFORMA] IN ('A', 'I')");
                t.HasCheckConstraint("CK_NBMTB001_NU_VERSAO_SO", "[NU_VERSAO_SISTEMA_OPERACIONAL] > 0");
            });
        });

        modelBuilder.Entity<Nbmtb002ModeloDevice>(entity =>
        {
            entity.HasKey(e => e.CoModeloDevice);

            entity.ToTable(DatabaseConstants.TableNames.NBMTB002_MODELO_DEVICE);

            entity.HasIndex(e => e.DeModeloDevice, DatabaseConstants.IndexNames.IX_NBMTB002_MODELO_DEVICE).IsUnique();

            entity.Property(e => e.CoModeloDevice)
                .ValueGeneratedNever()
                .HasColumnName(DatabaseConstants.ColumnNames.CO_MODELO_DEVICE);
            entity.Property(e => e.CoPlataforma)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName(DatabaseConstants.ColumnNames.CO_PLATAFORMA);
            entity.Property(e => e.DeModeloDevice)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName(DatabaseConstants.ColumnNames.DE_MODELO_DEVICE);
        });

        modelBuilder.Entity<Nbmtb003VersaoAplicativo>(entity =>
        {
            entity.HasKey(e => e.CoVersaoAplicativo);

            entity.ToTable(DatabaseConstants.TableNames.NBMTB003_VERSAO_APLICATIVO);

            entity.HasIndex(e => e.NuVersaoAplicativo, DatabaseConstants.IndexNames.IX_NBMTB003_VERSAO_APLICATIVO).IsUnique();

            entity.Property(e => e.CoVersaoAplicativo)
                .ValueGeneratedNever()
                .HasColumnName(DatabaseConstants.ColumnNames.CO_VERSAO_APLICATIVO);
            entity.Property(e => e.CoPlataforma)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName(DatabaseConstants.ColumnNames.CO_PLATAFORMA);
            entity.Property(e => e.NuVersaoAplicativo)
                .HasColumnType("decimal(6, 3)")
                .HasColumnName(DatabaseConstants.ColumnNames.NU_VERSAO_APLICATIVO);

            entity.HasMany(d => d.CoVersaoMiniapps).WithMany(p => p.CoVersaoAplicativos)
                .UsingEntity<Dictionary<string, object>>(
                    "Nbmtb008VersaoMiniappVsAplicativo",
                    r => r.HasOne<Nbmtb006VersaoMiniapp>().WithMany()
                        .HasForeignKey(DatabaseConstants.ForeignKeyProperties.CoVersao)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName(DatabaseConstants.ForeignKeyNames.FK_NBMTB008_VS_MINIAPP_VS_APLICATIVO_NBMTB006_VERSAO_MINIAPP),
                    l => l.HasOne<Nbmtb003VersaoAplicativo>().WithMany()
                        .HasForeignKey(DatabaseConstants.ForeignKeyProperties.CoVersaoAplicativo)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName(DatabaseConstants.ForeignKeyNames.FK_NBMTB008_VS_MINIAPP_VS_APLICATIVO_NBMTB003_VERSAO_APLICATIVO),
                    j =>
                    {
                        j.HasKey(DatabaseConstants.ForeignKeyProperties.CoVersaoAplicativo, DatabaseConstants.ForeignKeyProperties.CoVersao);
                        j.ToTable(DatabaseConstants.TableNames.NBMTB008_VERSAO_MINIAPP_VS_APLICATIVO);
                        j.IndexerProperty<Guid>(DatabaseConstants.ForeignKeyProperties.CoVersaoAplicativo).HasColumnName(DatabaseConstants.ColumnNames.CO_VERSAO_APLICATIVO);
                        j.IndexerProperty<Guid>(DatabaseConstants.ForeignKeyProperties.CoVersao).HasColumnName(DatabaseConstants.ColumnNames.CO_VERSAO_MINIAPP);
                    });
        });

        modelBuilder.Entity<Nbmtb004Miniapp>(entity =>
        {
            entity.HasKey(e => e.CoMiniapp);

            entity.ToTable(DatabaseConstants.TableNames.NBMTB004_MINIAPP);

            entity.HasIndex(e => e.NoMiniapp, DatabaseConstants.IndexNames.IX_NBMTB004_MINIAPP).IsUnique();

            entity.Property(e => e.CoMiniapp)
                .ValueGeneratedNever()
                .HasColumnName(DatabaseConstants.ColumnNames.CO_MINIAPP);
            entity.Property(e => e.DeMiniapp)
                .HasColumnType("text")
                .HasColumnName(DatabaseConstants.ColumnNames.DE_MINIAPP);

            entity.Property(e => e.IcAtivo)
                .HasDefaultValue(true)
                .HasColumnName(DatabaseConstants.ColumnNames.IC_ATIVO);
            entity.Property(e => e.IcMiniappInicial).HasColumnName(DatabaseConstants.ColumnNames.IC_MINIAPP_INICIAL);
            entity.Property(e => e.NoApelidoMiniapp)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName(DatabaseConstants.ColumnNames.NO_APELIDO_MINIAPP);
            entity.Property(e => e.NoMiniapp)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName(DatabaseConstants.ColumnNames.NO_MINIAPP);
        });

        modelBuilder.Entity<Nbmtb005RelacionamentoMiniapp>(entity =>
        {
            entity.HasKey(e => new { e.CoMiniappPai, e.CoMiniappFilho });

            entity.ToTable(DatabaseConstants.TableNames.NBMTB005_RELACIONAMENTO_MINIAPP);

            entity.Property(e => e.CoMiniappPai).HasColumnName(DatabaseConstants.ColumnNames.CO_MINIAPP_PAI);
            entity.Property(e => e.CoMiniappFilho).HasColumnName(DatabaseConstants.ColumnNames.CO_MINIAPP_FILHO);

            entity.HasOne<Nbmtb004Miniapp>()
                .WithMany()
                .HasForeignKey(e => e.CoMiniappPai)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(DatabaseConstants.ForeignKeyNames.FK_NBMTB005_RELACIONAMENTO_MINIAPP_NBMTB004_MINIAPP_PAI);

            entity.HasOne<Nbmtb004Miniapp>()
                .WithMany()
                .HasForeignKey(e => e.CoMiniappFilho)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(DatabaseConstants.ForeignKeyNames.FK_NBMTB005_RELACIONAMENTO_MINIAPP_NBMTB004_MINIAPP_FILHO);
        });

        modelBuilder.Entity<Nbmtb006VersaoMiniapp>(entity =>
        {
            entity.HasKey(e => e.CoVersaoMiniapp);

            entity.ToTable(DatabaseConstants.TableNames.NBMTB006_VERSAO_MINIAPP);

            entity.HasIndex(e => new { e.CoMiniapp, e.NuVersaoMiniapp }, DatabaseConstants.IndexNames.IX_NBMTB006_VERSAO_MINIAPP).IsUnique();

            entity.Property(e => e.CoVersaoMiniapp)
                .ValueGeneratedNever()
                .HasColumnName(DatabaseConstants.ColumnNames.CO_VERSAO_MINIAPP);
            entity.Property(e => e.CoMiniapp).HasColumnName(DatabaseConstants.ColumnNames.CO_MINIAPP);
            entity.Property(e => e.DhFimVigencia)
                .HasColumnType("datetime")
                .HasColumnName(DatabaseConstants.ColumnNames.DH_FIM_VIGENCIA);
            entity.Property(e => e.DhInicioVigencia)
                .HasColumnType("datetime")
                .HasColumnName(DatabaseConstants.ColumnNames.DH_INICIO_VIGENCIA);
            entity.Property(e => e.EdVersaoMiniapp)
               .HasMaxLength(200)
               .IsUnicode(false)
               .HasColumnName(DatabaseConstants.ColumnNames.ED_VERSAO_MINIAPP);
            entity.Property(e => e.IcAtivo)
                .HasDefaultValue(true)
                .HasColumnName(DatabaseConstants.ColumnNames.IC_ATIVO);
            entity.Property(e => e.NuVersaoMiniapp)
                .HasColumnType("decimal(6, 3)")
                .HasColumnName(DatabaseConstants.ColumnNames.NU_VERSAO_MINIAPP);
            entity.Property(e => e.PcExpansaoMiniapp)
                .HasColumnType("decimal(4, 3)")
                .HasColumnName(DatabaseConstants.ColumnNames.PC_EXPANSAO_MINIAPP);

            entity.HasOne(d => d.CoMiniappNavigation).WithMany(p => p.Nbmtb006VersaoMiniapps)
                .HasForeignKey(d => d.CoMiniapp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(DatabaseConstants.ForeignKeyNames.FK_NBMTB006_VERSAO_MINIAPP_NBMTB004_MINIAPP);

            entity.HasMany(d => d.CoModeloDevices).WithMany(p => p.CoVersaoMiniapps)
                .UsingEntity<Dictionary<string, object>>(
                    "Nbmtb009VersaoMiniappModeloDevice",
                    r => r.HasOne<Nbmtb002ModeloDevice>().WithMany()
                        .HasForeignKey(DatabaseConstants.ForeignKeyProperties.CoModeloDevice)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName(DatabaseConstants.ForeignKeyNames.FK_NBMTB009_VS_MINIAPP_MODELO_DEVICE_NBMTB002_MODELO_DEVICE),
                    l => l.HasOne<Nbmtb006VersaoMiniapp>().WithMany()
                        .HasForeignKey(DatabaseConstants.ForeignKeyProperties.CoVersao)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName(DatabaseConstants.ForeignKeyNames.FK_NBMTB009_VS_MINIAPP_MODELO_DEVICE_NBMTB006_VERSAO_MINIAPP),
                    j =>
                    {
                        j.HasKey(DatabaseConstants.ForeignKeyProperties.CoVersao, DatabaseConstants.ForeignKeyProperties.CoModeloDevice);
                        j.ToTable(DatabaseConstants.TableNames.NBMTB009_VERSAO_MINIAPP_MODELO_DEVICE);
                        j.IndexerProperty<Guid>(DatabaseConstants.ForeignKeyProperties.CoVersao).HasColumnName(DatabaseConstants.ColumnNames.CO_VERSAO_MINIAPP);
                        j.IndexerProperty<Guid>(DatabaseConstants.ForeignKeyProperties.CoModeloDevice).HasColumnName(DatabaseConstants.ColumnNames.CO_MODELO_DEVICE);
                    });

            entity.HasMany(d => d.CoVersaoSistemaOperacionals).WithMany(p => p.CoVersaoMiniapps)
                .UsingEntity<Dictionary<string, object>>(
                    "Nbmtb007VersaoMiniappVsSistemaOperacional",
                    r => r.HasOne<Nbmtb001VersaoSistemaOperacional>().WithMany()
                        .HasForeignKey(DatabaseConstants.ForeignKeyProperties.CoVersaoSistemaOperacional)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName(DatabaseConstants.ForeignKeyNames.FK_NBMTB007_VS_MINIAPP_VS_SISTEMA_OPERACIONAL_NBMTB001_VERSAO_SISTEMA_OPERACIONAL),
                    l => l.HasOne<Nbmtb006VersaoMiniapp>().WithMany()
                        .HasForeignKey(DatabaseConstants.ForeignKeyProperties.CoVersaoMiniapp)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName(DatabaseConstants.ForeignKeyNames.FK_NBMTB007_VS_MINIAPP_VS_SISTEMA_OPERACIONAL_NBMTB006_VERSAO_MINIAPP),
                    j =>
                    {
                        j.HasKey(DatabaseConstants.ForeignKeyProperties.CoVersaoMiniapp, DatabaseConstants.ForeignKeyProperties.CoVersaoSistemaOperacional);
                        j.ToTable(DatabaseConstants.TableNames.NBMTB007_VERSAO_MINIAPP_VS_SISTEMA_OPERACIONAL);
                        j.IndexerProperty<Guid>(DatabaseConstants.ForeignKeyProperties.CoVersaoMiniapp).HasColumnName(DatabaseConstants.ColumnNames.CO_VERSAO_MINIAPP);
                        j.IndexerProperty<Guid>(DatabaseConstants.ForeignKeyProperties.CoVersaoSistemaOperacional).HasColumnName(DatabaseConstants.ColumnNames.CO_VERSAO_SISTEMA_OPERACIONAL);
                    });
        });

        modelBuilder.Entity<Nbmtb010UsuarioVersaoMiniapp>(entity =>
        {
            entity.HasKey(e => e.CoUsuarioVersaoMiniapp);

            entity.ToTable(DatabaseConstants.TableNames.NBMTB010_USUARIO_VERSAO_MINIAPP);

            entity.HasIndex(e => e.NuCpfUsuario, DatabaseConstants.IndexNames.IX_NBMTB010_USUARIO_VERSAO_MINIAPP);

            entity.Property(e => e.CoUsuarioVersaoMiniapp)
                .ValueGeneratedNever()
                .HasColumnName(DatabaseConstants.ColumnNames.CO_USUARIO_VERSAO_MINIAPP);
            entity.Property(e => e.CoVersaoMiniapp).HasColumnName(DatabaseConstants.ColumnNames.CO_VERSAO_MINIAPP);
            entity.Property(e => e.NuCpfUsuario)
                .HasColumnType("decimal(11, 0)")
                .HasColumnName(DatabaseConstants.ColumnNames.NU_CPF_USUARIO);

            entity.HasOne(d => d.CoVersaoMiniappNavigation).WithMany(p => p.Nbmtb010UsuarioVersaoMiniapps)
                .HasForeignKey(d => d.CoVersaoMiniapp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(DatabaseConstants.ForeignKeyNames.FK_NBMTB010_USUARIO_VS_MINIAPP_NBMTB006_VERSAO_MINIAPP);
        });

    }

}
