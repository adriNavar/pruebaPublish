using GeoSit.Data.BusinessEntities.Via;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoViaMapper : AuditoriaEntityTypeConfiguration<TipoVia>
    {
        public TipoViaMapper() :base()
        {
            this.ToTable("GRF_TIPO_VIA")
                .HasKey(a => a.TipoViaId);

            this.Property(a => a.TipoViaId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_TIPO_VIA");

            this.Property(a => a.Nombre)
                .HasColumnName("DESCRIPCION")
                .IsRequired();
        }
    }
}
