using GeoSit.Data.BusinessEntities.ModuloPloteo;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.Ploteo
{
    public class UbicacionPloteoMapper : EntityTypeConfiguration<UbicacionPloteo>
    {
        public UbicacionPloteoMapper()
        {
            this.ToTable("RC_UBICACION_PLOTEO");

            this.Property(a => a.IdUbicacionPloteo)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_UBICACION_PLOTEO");
            this.Property(a => a.Distrito)
                .IsRequired()
                .HasColumnName("DISTRITO");
            this.Property(a => a.Calle)
                .IsRequired()
                .HasColumnName("CALLE");
            this.Property(a => a.Altura)
                .HasColumnName("ALTURA");
            this.Property(a => a.Interseccion)
                .HasColumnName("CALLE2");
            this.Property(a => a.Manzana)
                .IsRequired()
                .HasColumnName("MANZANA");
            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.HasKey(a => a.IdUbicacionPloteo);
        }
    }
}
