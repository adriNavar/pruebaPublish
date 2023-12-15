using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class VALDecretoZonaMapper : EntityTypeConfiguration<VALDecretoZona>
    {
        public VALDecretoZonaMapper()
        {
            this.ToTable("VAL_DEC_ZONA");

            this.HasKey(a => a.IdDecretoZona);

            this.Property(a => a.IdDecretoZona)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DEC_ZONA");

            this.Property(a => a.IdDecreto)
               .IsRequired()
               .HasColumnName("ID_DECRETO");

            this.Property(a => a.IdTipoParcela)
                .IsRequired()
                .HasColumnName("ID_TIPO_PARCELA");                            

            this.Property(a => a.IdUsuarioAlta)
               .IsRequired()
               .HasColumnName("ID_USU_ALTA");            

            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.IdUsuarioModif)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            this.Property(a => a.FechaModif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.IdUsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            this.HasRequired(a => a.Decreto)
                .WithMany(a => a.Zona)
                .HasForeignKey(a => a.IdDecreto);

            this.HasRequired(a => a.TipoParcela)
                .WithMany()
                .HasForeignKey(a => a.IdTipoParcela);
        }
    }
}
