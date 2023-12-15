using GeoSit.Data.BusinessEntities.Seguridad;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class UsuariosPerfilesMapper : EntityTypeConfiguration<UsuariosPerfiles>
    {
        public UsuariosPerfilesMapper()
        {
            this.ToTable("SE_USUARIO_PERFIL");

            this.Property(a => a.Id_Usuario_Perfil)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_USUARIO_PERFIL");
            this.Property(a => a.Id_Usuario)
                .IsRequired()
                .HasColumnName("ID_USUARIO");
            this.Property(a => a.Id_Perfil)
                .IsRequired()
                .HasColumnName("ID_PERFIL");
            this.Property(a => a.Id_Horario)
                .IsRequired()
                .HasColumnName("ID_HORARIO");
            this.Property(a => a.Usuario_Alta)
                .IsRequired()
                .IsConcurrencyToken()
                .HasColumnName("USUARIO_ALTA");
            this.Property(a => a.Fecha_Alta)
                .IsRequired()
                .IsConcurrencyToken()
                .HasColumnName("FECHA_ALTA");
            this.Property(a => a.Usuario_Baja)
                .HasColumnName("USUARIO_BAJA");
            this.Property(a => a.Fecha_Baja)
                .IsConcurrencyToken()
                .HasColumnName("FECHA_BAJA");

            this.HasKey(a => a.Id_Usuario_Perfil);
        }
    }
}
