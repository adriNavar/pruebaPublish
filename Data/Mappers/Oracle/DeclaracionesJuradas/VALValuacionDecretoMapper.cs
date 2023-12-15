using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class VALValuacionDecretoMapper : EntityTypeConfiguration<VALValuacionDecreto>
    {
        public VALValuacionDecretoMapper()
        {
            this.ToTable("VAL_VAL_DECRETOS");

            //this.HasKey(a => a.IdValuacionDecreto);

            this.HasKey(a => new { a.IdValuacion, a.IdDecreto });

            this.Property(a => a.IdValuacionDecreto)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_VAL_DECRETO");

            this.Property(a => a.IdValuacion)
               .IsRequired()
               .HasColumnName("ID_VALUACION");

            this.Property(a => a.IdDecreto)
                .IsRequired()
                .HasColumnName("ID_DECRETO");          

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


            this.HasRequired(a => a.Valuacion)
              .WithMany(a=> a.ValuacionDecretos)
              .HasForeignKey(a => a.IdValuacion);

            this.HasRequired(a => a.Decreto)
              .WithMany(a=> a.ValuacionesDecreto)
              .HasForeignKey(a => a.IdDecreto);

        }
    }
}
