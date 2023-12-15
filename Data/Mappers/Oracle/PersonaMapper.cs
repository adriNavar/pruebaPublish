using GeoSit.Data.BusinessEntities.Personas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class PersonaMapper : EntityTypeConfiguration<Persona>
    {
        public PersonaMapper()
        {
            this.ToTable("INM_PERSONA");

            this.Property(a => a.PersonaId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_PERSONA");
            this.Property(a => a.TipoDocId)
                .IsRequired()
                .HasColumnName("ID_TIPO_DOC_IDENT");
            this.Property(a => a.NroDocumento)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("NRO_DOCUMENTO");
            this.Property(a => a.TipoPersonaId)
                .IsRequired()
                .HasColumnName("ID_TIPO_PERSONA");
            this.Property(a => a.NombreCompleto)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NOMBRE_COMPLETO");
            this.Property(a => a.Nombre)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
            this.Property(a => a.Apellido)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("APELLIDO");
            this.Property(a => a.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");
            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");
            this.Property(a => a.UsuarioModifId)
                .HasColumnName("ID_USU_MODIF");
            this.Property(a => a.FechaModif)
                .HasColumnName("FECHA_MODIF");
            this.Property(a => a.UsuarioBajaId)
                .HasColumnName("ID_USU_BAJA");
            this.Property(a => a.FechaBaja)
                .IsOptional()
                .HasColumnName("FECHA_BAJA");
            this.Property(a => a.Sexo)
                .HasColumnName("SEXO");
            this.Property(a => a.EstadoCivil)
                .HasColumnName("ESTADOCIVIL");
            this.Property(a => a.Nacionalidad)
                .HasColumnName("NACIONALIDAD");
            this.Property(a => a.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("TELEFONO");
            this.Property(a => a.Email)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("EMAIL");

            this.HasKey(a => a.PersonaId);


            //Relaciones
            this.HasRequired(p => p.TipoPersona)
                .WithMany(tp => tp.Personas)
                .HasForeignKey(p => p.TipoPersonaId);

            this.HasRequired(p => p.TipoDocumentoIdentidad)
                .WithMany()
                .HasForeignKey(p => p.TipoDocId);

            this.HasMany(p => p.PersonaDomicilios)
                .WithRequired()
                .HasForeignKey(p => p.PersonaId);
        }

    }

}
