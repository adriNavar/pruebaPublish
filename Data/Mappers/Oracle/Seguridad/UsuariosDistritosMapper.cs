using GeoSit.Data.BusinessEntities.Seguridad;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class UsuariosDistritosMapper : EntityTypeConfiguration<UsuariosDistritos>
    {
        public UsuariosDistritosMapper()
        {
            this.ToTable("SE_USUARIO_DISTRITO");

            this.Property(a => a.Id_Usuario_Distrito)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_USUARIO_DISTRITO");
            this.Property(a => a.Id_Usuario)
                .IsRequired()
                .HasColumnName("ID_USUARIO");
            this.Property(a => a.Id_Distrito)
                .IsRequired()
                .HasColumnName("ID_DISTRITO");
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

            this.HasKey(a => a.Id_Usuario_Distrito);
        }
    }
}
