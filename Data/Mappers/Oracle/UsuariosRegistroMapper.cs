using GeoSit.Data.BusinessEntities.Seguridad;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class UsuariosRegistroMapper : EntityTypeConfiguration<UsuariosRegistro>
    {
        public UsuariosRegistroMapper()
        {
            this.ToTable("SE_USUARIO_REGISTRO");

            this.Property(a => a.Id_Usuario_Registro)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_USUARIO_REGISTRO");
            this.Property(a => a.Id_Usuario)
                .IsRequired()
                .HasColumnName("ID_USUARIO");
            this.Property(a => a.Registro)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("REGISTRO");
            this.Property(a => a.Usuario_Operacion)
                .IsRequired()
                .HasColumnName("USUARIO_OPERACION");

            this.Property(a => a.Fecha_Operacion)
                .IsConcurrencyToken()
                .HasColumnName("FECHA_OPERACION");

            this.HasKey(a => a.Id_Usuario_Registro);
        }
    }
}
