using GeoSit.Data.BusinessEntities.Redes;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public abstract class RelacionMapper<TEntityType> : EntityTypeConfiguration<TEntityType> where TEntityType : Relacion
    {
        public RelacionMapper()
        {

            this.Property(a => a.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_RELACION");

            this.Property(a => a.TablaPadre)
                .HasColumnName("TABLA_PADRE");
            this.Property(a => a.IdTablaPadre)
                .HasColumnName("ID_TABLA_PADRE");
            this.Property(a => a.TablaHijo)
                .HasColumnName("TABLA_HIJO");
            this.Property(a => a.IdTablaHijo)
                .HasColumnName("ID_TABLA_HIJO");
            this.Property(a => a.UsuarioAlta)
                .HasColumnName("USUARIO_ALTA");
            this.Property(a => a.FechaAlta)
                .HasColumnName("FECHA_ALTA");
            this.Property(a => a.UsuarioModif)
                .HasColumnName("USUARIO_MODIFICACION");
            this.Property(a => a.FechaModif)
                .HasColumnName("FECHA_MODIFICACION");
            this.Property(a => a.UsuarioBaja)
                .HasColumnName("USUARIO_BAJA");
            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");
            
            this.HasKey(a => a.Id);
        }
    }
}
