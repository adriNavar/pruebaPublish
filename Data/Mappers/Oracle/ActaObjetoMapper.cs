using GeoSit.Data.BusinessEntities.ObrasParticulares;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ActaObjetoMapper : EntityTypeConfiguration<ActaObjeto>
    {
        public ActaObjetoMapper()
        {
            this.ToTable("OP_ACTA_OBJETO");

            this.Property(a => a.ActaID)
                .IsRequired()
                .HasColumnName("ID_ACTA");
            this.Property(a => a.id_tipo_objeto)
                .IsRequired()
                .HasColumnName("ID_TIPO_OBJETO");
            this.Property(a => a.id_objeto)
                .IsRequired()
                .HasColumnName("NRO_OBJETO");

            this.HasKey(a => new { a.ActaID, a.id_tipo_objeto, a.id_objeto });

            HasRequired(a => a.Acta)
                .WithMany()
                .HasForeignKey(a => a.ActaID);
        }
    }
}
