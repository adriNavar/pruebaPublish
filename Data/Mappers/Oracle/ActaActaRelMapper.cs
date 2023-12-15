using GeoSit.Data.BusinessEntities.Actas;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ActaActaRelMapper : EntityTypeConfiguration<ActaActaRel>
    {
        public ActaActaRelMapper()
        {
            this.ToTable("OP_ACTA_REL");

            this.Property(a => a.ActaId)
                .IsRequired()
                .HasColumnName("ID_ACTA");

            this.Property(a => a.ActaRelId)
                .IsRequired()
                .HasColumnName("ID_ACTA_REL");


            this.HasKey(a => new { a.ActaId, a.ActaRelId });

            HasRequired(a => a.Acta)
                .WithMany()
                .HasForeignKey(a => a.ActaRelId);
        }
    }
}
