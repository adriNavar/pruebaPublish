using GeoSit.Data.BusinessEntities.ObrasParticulares;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ActaPersonasMapper : EntityTypeConfiguration<ActaPersonas>
    {
        public ActaPersonasMapper()
        {
            this.ToTable("OP_ACTA_PERSONA");

            this.Property(a => a.PersonaRolId)
                .IsRequired()
                .HasColumnName("ID_ACTA_ROL_PERSONA");
            this.Property(a => a.ActaId)
                .IsRequired()
                .HasColumnName("ID_ACTA");
            this.Property(a => a.PersonaId)
                .IsRequired()
                .HasColumnName("ID_PERSONA");

            this.HasKey(a => new { a.ActaId, a.PersonaId, a.PersonaRolId });

            this.HasRequired(a => a.Persona).WithMany().HasForeignKey(a => a.PersonaId);
            
            HasRequired(a => a.Acta)
                .WithMany()
                .HasForeignKey(a => a.ActaId);
        }
    }
}
