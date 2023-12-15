using GeoSit.Data.BusinessEntities.Seguridad;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class UsuariosHistMapper : EntityTypeConfiguration<UsuariosHist>
    {
        public UsuariosHistMapper()
        {
            this.ToTable("SE_USUARIO_HIST");

            this.Property(a => a.Id_Usuario_Hist)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_USUARIO_HIST");
            this.Property(a => a.Id_Usuario)
                .IsRequired()
                .HasColumnName("ID_USUARIO");
            this.Property(a => a.Login)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LOGIN");
            this.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
            this.Property(a => a.Apellido)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("APELLIDO");

            this.Property(a => a.Mail)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MAIL");

            this.Property(a => a.Sector)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("SECTOR");

            this.Property(a => a.Id_Tipo_Doc)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID_TIPO_DOC");

            this.Property(a => a.Nro_Doc)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("NRO_DOC");

            this.Property(a => a.Domicilio)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("DOMICILIO");

            this.Property(a => a.Habilitado)
                .IsRequired()
                .HasColumnName("HABILITADO");

            this.Property(a => a.Cambio_Pass)
                .HasColumnName("CAMBIO_PASS");

            this.Property(a => a.Usuario_Operacion)
                .IsRequired()
                .HasColumnName("USUARIO_OPERACION");

            this.Property(a => a.Fecha_Operacion)
                .IsRequired()
                .IsConcurrencyToken()
                .HasColumnName("FECHA_OPERACION");


            this.HasKey(a => a.Id_Usuario_Hist);
        }
    }
}
