using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.Actas;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ActaEstadoMapper : EntityTypeConfiguration<ActaEstado>
    {
        public ActaEstadoMapper()
        {
            ToTable("OP_ACTA_ESTADO");

            this.Property(a => a.EstadoActaId)
                .IsRequired()
                .HasColumnName("ID_ESTADO_ACTA");

            this.Property(a => a.ActaId)
                .IsRequired()
                .HasColumnName("ID_ACTA");

            this.HasKey(a => new { a.EstadoActaId, a.ActaId });

            HasRequired(a => a.Acta)
                .WithMany()
                .HasForeignKey(a => a.ActaId);
        }
    }
}
