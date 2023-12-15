using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.DeclaracionesJuradas
{
    public class INMMejoraMapper : EntityTypeConfiguration<INMMejora>
    {
        public INMMejoraMapper()
        {
            this.ToTable("INM_MEJORAS");

            this.HasKey(a => a.IdMejora);

            this.Property(a => a.IdMejora)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_MEJORA");

            this.Property(a => a.IdEstadoConservacion)
               .HasColumnName("ID_ESTADO_CONSERVACION");

            this.Property(a => a.IdDestinoMejora)            
                .HasColumnName("ID_DESTINO_MEJORA");

            this.Property(a => a.IdDeclaracionJurada)
                .IsRequired()
                .HasColumnName("ID_DDJJ");

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

            this.HasRequired(a => a.EstadoConservacion)
               .WithMany()
               .HasForeignKey(a => a.IdEstadoConservacion);

            this.HasRequired(a => a.DeclaracionJurada)
                .WithMany(a=> a.Mejora)
                .HasForeignKey(a => a.IdDeclaracionJurada);

            /*this.HasRequired(a => a.MejorasCar)
                .WithMany()
                .HasForeignKey(a => a.IdMejora);*/
        }      

    }
}
