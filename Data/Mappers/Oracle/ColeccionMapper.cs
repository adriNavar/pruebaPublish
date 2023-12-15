using GeoSit.Data.BusinessEntities.MapasTematicos;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ColeccionMapper : EntityTypeConfiguration<Coleccion>
    {
        public ColeccionMapper()
        {
            this.ToTable("GE_COLECCION");

            this.Property(a => a.ColeccionId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_COLECCION");
            this.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
            this.Property(a => a.UsuarioAlta)
                .IsRequired()
                .HasColumnName("USUARIO_ALTA");
            this.Property(a => a.UsuarioModificacion)
                .IsRequired()
                .HasColumnName("USUARIO_MODIF");
            this.Property(a => a.UsuarioBaja)
                .HasColumnName("USUARIO_BAJA");
            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");
            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");
            this.Property(a => a.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            this.HasKey(a => a.ColeccionId);

            this.HasMany(a => a.Componentes)
                .WithRequired(p => p.Coleccion)
                .HasForeignKey(k => k.ColeccionId);

        }
    }
}
