using GeoSit.Data.BusinessEntities.Personas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class PersonaDomicilioMapper : EntityTypeConfiguration<PersonaDomicilio>
    {
        public PersonaDomicilioMapper()
        {
            this.ToTable("INM_PERSONA_DOMICILIO");

            this.Property(a => a.PersonaId)
                .IsRequired()
                .HasColumnName("ID_PERSONA");
            this.Property(a => a.DomicilioId)
                .IsRequired()
                .HasColumnName("ID_DOMICILIO");
            this.Property(a => a.TipoDomicilioId)
               .IsRequired()
               .HasColumnName("ID_TIPO_DOMICILIO");
            this.Property(a => a.UsuarioAltaId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_USU_ALTA");
            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");
            this.Property(a => a.UsuarioModifId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_USU_MODIF");
            this.Property(a => a.FechaModif)
                .HasColumnName("FECHA_MODIF");
            this.Property(a => a.UsuarioBajaId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_USU_BAJA");
            this.Property(a => a.FechaBaja)
                .IsOptional()
                .HasColumnName("FECHA_BAJA");

            this.HasKey(a => new { a.PersonaId , a.DomicilioId, a.TipoDomicilioId});

            this.HasRequired(a => a.Domicilio).WithMany().HasForeignKey(a => a.DomicilioId);
            /*this.HasRequired()
                .WithMany(a => a.PersonaDomicilios)
                .HasForeignKey(a => a.PersonaId);*/
        }
    }
}
