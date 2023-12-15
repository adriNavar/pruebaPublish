using GeoSit.Data.BusinessEntities.ObrasParticulares;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class EstadoInspeccionMapper : EntityTypeConfiguration<EstadoInspeccion>
    {
        public EstadoInspeccionMapper()
        {
            this.ToTable("INM_ESTADO_INSPECCION");

            this.Property(a => a.EstadoInspeccionID)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_ESTADO_INSPECCION");
            this.Property(a => a.Descripcion)
                .IsRequired()
                .HasColumnName("DESCRIPCION")
                .HasMaxLength(50)
                .IsUnicode(false);

            this.HasKey(a => a.EstadoInspeccionID);
        }
    }
}
