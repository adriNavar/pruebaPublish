using GeoSit.Data.BusinessEntities.Personas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ProfesionMapper : EntityTypeConfiguration<Profesion>
    {
        public ProfesionMapper()
        {
            this.ToTable("INM_PROFESION");

            this.Property(a => a.PersonaId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_PERSONA");
            this.Property(a => a.TipoProfesionId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_TIPO_PROFESION"); 
            this.Property(a => a.Matricula)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MATRICULA");
 
            this.HasKey(a => a.PersonaId);
            this.HasKey(a => a.TipoProfesionId);
        }

    }
}
