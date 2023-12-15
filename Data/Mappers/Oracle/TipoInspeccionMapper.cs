using GeoSit.Data.BusinessEntities.ObrasParticulares;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoInspeccionMapper : EntityTypeConfiguration<TipoInspeccion>
    {
        public TipoInspeccionMapper()
        {
            this.ToTable("INM_TIPO_INSPECCION");

            this.Property(a => a.TipoInspeccionID)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_TIPO_INSPECCION");
            this.Property(a => a.Descripcion)
                .IsRequired()
                .HasColumnName("DESCRIPCION")
                .HasMaxLength(50)
                .IsUnicode(false);

            this.HasKey(a => a.TipoInspeccionID);
        }
    }
}
