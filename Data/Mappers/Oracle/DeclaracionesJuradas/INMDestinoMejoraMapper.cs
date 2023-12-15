using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.DeclaracionesJuradas
{
    public class INMDestinoMejoraMapper : EntityTypeConfiguration<INMDestinoMejora>
    {
        public INMDestinoMejoraMapper()
        {
            this.ToTable("INM_DESTINOS_MEJORAS");

            this.HasKey(a => a.IdDestinoMejora);

            this.Property(a => a.IdDestinoMejora)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DESTINO_MEJORA");

            this.Property(a => a.IdVersion)
               .HasColumnName("ID_DDJJ_VERSION");

            this.Property(a => a.Descripcion)            
                .HasColumnName("DESCRIPCION");           

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
