using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoInscripcionMapper : EntityTypeConfiguration<TipoInscripcion>
    {
        public TipoInscripcionMapper()
        {
            ToTable("INM_TIPO_INSCRIPCION");

            HasKey(ti => ti.TipoInscripcionID);

            Property(ti => ti.TipoInscripcionID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .IsRequired()
                .HasColumnName("ID_TIPO_INSCRIPCION");

            Property(ti => ti.Descripcion)
                .IsRequired()
                .HasColumnName("DESCRIPCION");

            Property(ti => ti.ExpresionRegular)
                .IsRequired()
                .HasColumnName("EXPRESION_REGULAR");
        }
    }
}
