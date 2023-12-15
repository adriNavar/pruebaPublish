using GeoSit.Data.BusinessEntities.Inmuebles;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class VIRVbEuUsosMapper : EntityTypeConfiguration<VIRVbEuUsos>
    {
        public VIRVbEuUsosMapper()
        {
            this.ToTable("VIR_VB_EU_USOS", "VIR_VALUACIONES")
                .HasKey(a => a.IdUso);

            this.Property(p => p.IdUso)
                .HasColumnName("ID_USO")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            this.Property(p => p.Uso)
                .HasColumnName("USO")
                .IsRequired();

            this.Property(p => p.Descripcion)
                .HasColumnName("DESCRIPCION")
                .IsRequired();

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
        }
    }
}
