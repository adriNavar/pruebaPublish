using GeoSit.Data.BusinessEntities.Seguridad;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class UsuariosMapper : EntityTypeConfiguration<Usuarios>
    {
        public UsuariosMapper()
        {
            this.ToTable("SE_USUARIO")
                .HasKey(a => a.Id_Usuario);

            this.Property(a => a.Id_Usuario)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
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

            this.Property(a => a.IdSector)
                .HasColumnName("ID_SECTOR");

            this.Property(a => a.Id_tipo_doc)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID_TIPO_DOC");

            this.Property(a => a.Nro_doc)
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

            this.Property(a => a.Cambio_pass)
                .HasColumnName("CAMBIO_PASS");

            this.Property(a => a.Usuario_alta)
                .IsRequired()
                .HasColumnName("USUARIO_ALTA");

            this.Property(a => a.Fecha_alta)
                .IsRequired()
                .IsConcurrencyToken()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.Usuario_modificacion)
                .IsRequired()
                .HasColumnName("USUARIO_MODIFICACION");

            this.Property(a => a.Fecha_modificacion)
                .IsRequired()
                .IsConcurrencyToken()
                .HasColumnName("FECHA_MODIFICACION");

            this.Property(a => a.Usuario_baja)
                .HasColumnName("USUARIO_BAJA");

            this.Property(a => a.Fecha_baja)
                .IsConcurrencyToken()
                .HasColumnName("FECHA_BAJA");

            this.Property(a => a.CantidadIngresosFallidos)
                .HasColumnName("CANT_INGR_FALLIDOS");

            this.Property(a => a.IdISICAT)
                .HasColumnName("ID_ISICAT")
                .IsOptional();

            this.Property(a => a.LoginISICAT)
                .HasColumnName("LOGIN_ISICAT")
                .IsOptional();

            this.Property(a => a.NombreApellidoISICAT)
                .HasColumnName("NOMBRE_APELLIDO_ISICAT")
                .IsOptional();

            this.Property(a => a.VigenciaDesdeISICAT)
                .HasColumnName("VIGENCIA_DESDE_ISICAT")
                .IsOptional();

            this.Property(a => a.VigenciaHastaISICAT)
                .HasColumnName("VIGENCIA_HASTA_ISICAT")
                .IsOptional();

            this.Ignore(a => a.Fecha_Operacion);
            this.Ignore(a => a.NombreApellidoCompleto);

            this.HasOptional(a => a.SectorUsuario)
                .WithMany()
                .HasForeignKey(a => a.IdSector);
        }
    }
}
