using GeoSit.Data.BusinessEntities.MapasTematicos;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ColeccionComponenteMapper : EntityTypeConfiguration<ColeccionComponente>
    {
        public ColeccionComponenteMapper()
        {

            this.ToTable("GE_COLEC_COMP");

            this.Property(a => a.ColeccionComponenteId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_COLEC_COMP");
            this.Property(a => a.ColeccionId)
                .IsRequired()
                .HasColumnName("ID_COLECCION");
            this.Property(a => a.ComponenteId)
                .IsRequired()
                .HasColumnName("ID_COMPONENTE");
            this.Property(a => a.ObjetoId)
                .IsRequired()
                .HasColumnName("ID_OBJETO");
            this.Property(a => a.UsuarioAlta)
                .IsRequired()
                .HasColumnName("USUARIO_ALTA");
            this.Property(a => a.UsuarioBaja)
                .HasColumnName("USUARIO_BAJA");
            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");
            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            this.HasKey(a => a.ColeccionComponenteId);
        }
    }
}
