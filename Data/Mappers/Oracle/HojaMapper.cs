using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.Mappers.Oracle
{
    public class HojaMapper : EntityTypeConfiguration<Hoja>
    {
        public HojaMapper()
        {
            //Table mapping
            ToTable("MP_HOJA");

            //Primary key
            HasKey(h => h.IdHoja);

            //Properties
            Property(h => h.IdHoja)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_HOJA");

            Property(h => h.Nombre)
                .IsRequired()
                .HasColumnName("NOMBRE");

            Property(h => h.Alto_mm)
                .IsRequired()
                .HasColumnName("ALTO_MM");

            Property(h => h.Ancho_mm)
                .IsRequired()
                .HasColumnName("ANCHO_MM");

            Property(h => h.Ancho_Imagen_px)
                .HasColumnName("ANCHO_IMAGEN_PX");

        }
    }
}
