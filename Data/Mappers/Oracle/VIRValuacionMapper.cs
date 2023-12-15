using GeoSit.Data.BusinessEntities.Inmuebles;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class VIRValuacionMapper : EntityTypeConfiguration<VIRValuacion>
    {
        public VIRValuacionMapper()
        {
            this.ToTable("VIR_VALUACIONES", "VIR_VALUACIONES")
                .HasKey(a => a.ValuacionId);

            this.Property(p => p.ValuacionId)
                .HasColumnName("ID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            this.Property(p => p.CorridaId)
                .HasColumnName("ID_CORRIDA")
                .IsRequired();

            this.Property(p => p.InmuebleId)
                .HasColumnName("ID_INMUEBLE")
                .IsRequired();

            this.Property(p => p.Partida)
                .HasColumnName("PARTIDA")
                .IsOptional();

            this.Property(p => p.VigenciaDesde)
                .HasColumnName("VIG_DESDE")
                .IsOptional();

            this.Property(p => p.Zona)
                .HasColumnName("VIR_ZONA")
                .IsOptional();

            this.Property(p => p.ValuacionTipo)
                .HasColumnName("VIR_TIPO_VALUACION")
                .IsOptional();

            this.Property(p => p.SuperficieTierra)
                .HasColumnName("VIR_SUP_TIERRA")
                .IsOptional();

            this.Property(p => p.UnidadMedidaSupTierra)
                .HasColumnName("VIR_SUP_TIERRA_UNID")
                .IsOptional();

            this.Property(p => p.ValorTierra)
                .HasColumnName("VIR_VAL_TIERRA")
                .IsOptional();

            this.Property(p => p.TipoMejoraUso)
                .HasColumnName("VIR_MEJ_USO_TIPO")
                .IsOptional();

            this.Property(p => p.SuperficieMejora)
                .HasColumnName("VIR_SUP_MEJORAS")
                .IsOptional();

            this.Property(p => p.UnidadMedidaSupMejora)
                .HasColumnName("VIR_SUP_MEJORAS_UNID")
                .IsOptional();

            this.Property(p => p.ValorMejoras)
                .HasColumnName("VIR_VAL_MEJORAS")
                .IsOptional();

            this.Property(p => p.ValorTotal)
                .HasColumnName("VIR_VAL_TOTAL")
                .IsOptional();

            this.Ignore(p => p.Vigente);
        }
    }
}
