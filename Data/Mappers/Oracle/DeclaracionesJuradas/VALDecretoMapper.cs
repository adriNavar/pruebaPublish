using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class VALDecretoMapper : EntityTypeConfiguration<VALDecreto>
    {
        public VALDecretoMapper()
        {
            this.ToTable("VAL_DECRETOS");

            this.HasKey(a => a.IdDecreto);

            this.Property(a => a.IdDecreto)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DECRETO");

            this.Property(a => a.NroDecreto)
               .IsRequired()
               .HasColumnName("NRO_DECRETO");

            this.Property(a => a.AnioDecreto)
                .IsRequired()
                .HasColumnName("ANIO_DECRETO");

            this.Property(a => a.FechaDecreto)
                .IsRequired()
                .HasColumnName("FECHA_DECRETO");


            this.Property(a => a.Coeficiente)
                .IsRequired()
                .HasColumnName("COEFICIENTE");

            this.Property(a => a.FechaInicio)
                .HasColumnName("FECHA_INICIO");

            this.Property(a => a.FechaFin)                
                .HasColumnName("FECHA_FIN");

            this.Property(a => a.Aplicado)
                .HasColumnName("APLICADO");          

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

        }
    }
}
