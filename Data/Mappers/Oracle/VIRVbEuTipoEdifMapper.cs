using GeoSit.Data.BusinessEntities.Inmuebles;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class VIRVbEuTipoEdifMapper : EntityTypeConfiguration<VIRVbEuTipoEdif>
    {
        public VIRVbEuTipoEdifMapper()
        {
            this.ToTable("VIR_VB_EU_TIPO_EDIF", "VIR_VALUACIONES")
                .HasKey(a => a.IdTipoEdif);

            this.Property(p => p.IdTipoEdif)
                .HasColumnName("ID_TIPO_EDIF")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            this.Property(p => p.IdTipo)
                .HasColumnName("ID_TIPO")
                .IsRequired();

            this.Property(p => p.TipoDescripcion)
                .HasColumnName("TIPO_DESCRIP")
                .IsOptional();

            this.Property(p => p.IdFuenteBase)
                .HasColumnName("ID_FUENTE_BASE")
                .IsOptional();

            this.Property(p => p.IdFuenteActualizadaAnual)
                .HasColumnName("ID_FUENTE_ACTUALIZ_ANUAL")
                .IsOptional();

            this.Property(p => p.CostoBase)
                .HasColumnName("COSTO_BASE")
                .IsOptional();

            this.Property(p => p.FechaBaja)
                .HasColumnName("FECHA_BAJA")
                .IsOptional();

            this.Property(p => p.IdUsuBaja)
                .HasColumnName("ID_USU_BAJA")
                .IsOptional();

            this.Property(p => p.FechaAlta)
                .HasColumnName("FECHA_ALTA")
                .IsOptional();

            this.Property(p => p.IdUsuAlta)
                .HasColumnName("ID_USU_ALTA")
                .IsOptional();

            this.Property(p => p.FechaModificacion)
                .HasColumnName("FECHA_MODIF")
                .IsOptional();

            this.Property(p => p.IdUsuModificacion)
                .HasColumnName("ID_USU_MODIF")
                .IsOptional();

            this.Property(p => p.IdUso)
                .HasColumnName("ID_USO")
                .IsOptional();
        }
    }
}
