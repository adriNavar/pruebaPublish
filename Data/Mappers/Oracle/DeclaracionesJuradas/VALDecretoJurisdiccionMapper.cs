using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class VALDecretoJurisdiccionMapper : EntityTypeConfiguration<VALDecretoJurisdiccion>
    {
        public VALDecretoJurisdiccionMapper()
        {
            this.ToTable("VAL_DEC_JURIS");

            this.HasKey(a => a.IdDecretoJurisdiccion);

            this.Property(a => a.IdDecretoJurisdiccion)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DEC_JURIS");

            this.Property(a => a.IdDecreto)
               .IsRequired()
               .HasColumnName("ID_DECRETO");

            this.Property(a => a.IdJurisdiccion)
                .IsRequired()
                .HasColumnName("ID_JURISDICCION");                            

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
               .WithMany(a => a.Jurisdiccion)
               .HasForeignKey(a => a.IdDecreto);
        }
    }
}
