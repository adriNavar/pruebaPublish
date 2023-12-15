using GeoSit.Data.BusinessEntities.Actas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class EstadoActaMapper : EntityTypeConfiguration<EstadoActa>
    {
        public EstadoActaMapper()
        {
            this.ToTable("OP_ESTADO_ACTA");

            this.Property(ea => ea.EstadoActaId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .IsRequired()
                .HasColumnName("ID_ESTADO_ACTA");

            this.Property(ea => ea.Descripcion)
                .IsRequired()
                .HasColumnName("DESCRIPCION");

            this.HasKey(a => a.EstadoActaId);
        }
    }
}
