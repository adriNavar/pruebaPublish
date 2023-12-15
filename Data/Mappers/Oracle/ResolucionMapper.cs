using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ResolucionMapper : EntityTypeConfiguration<Resolucion>
    {
        public ResolucionMapper()
        {
            //Table mapping
            ToTable("MP_RESOLUCION");

            //Primary key
            HasKey(r => r.IdResolucion);

            //Properties
            Property(r => r.IdResolucion)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_RESOLUCION");

            Property(pf => pf.Valor)
                .IsRequired()
                .HasColumnName("VALOR");
        }
    }
}
