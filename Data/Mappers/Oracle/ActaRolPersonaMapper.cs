using GeoSit.Data.BusinessEntities.Actas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ActaRolPersonaMapper : EntityTypeConfiguration<ActaRolPersona>
    {
        public ActaRolPersonaMapper()
        {
            this.ToTable("OP_ACTA_ROL_PERSONA");

            this.Property(a => a.ActaRolId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_ACTA_ROL_PERSONA");
            this.Property(a => a.Descripcion)
                .IsRequired()
                .HasColumnName("DESCRIPCION");


            this.HasKey(a => a.ActaRolId);
        }
    }
}
