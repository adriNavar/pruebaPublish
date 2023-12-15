using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.Mappers.Oracle
{
    public class NorteMapper: EntityTypeConfiguration<Norte>
    {
        public NorteMapper()
        {
            //Table mapping
            ToTable("MP_NORTE");

            //Primary key
            HasKey(n => n.IdNorte);

            //Properties
            Property(n => n.IdNorte)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_NORTE");

            Property(n => n.Nombre)
                .IsRequired()
                .HasColumnName("NOMBRE");

            Property(n => n.IBytes)
                .IsRequired()
                .HasColumnName("IMAGEN");

            Property(n => n.IType)
                .IsRequired()
                .HasColumnName("IMAGEN_TIPO");

            Property(n => n.AltoPx)
                .IsRequired()
                .HasColumnName("ALTO_PX");

            Property(n => n.AnchoPx)
                .IsRequired()
                .HasColumnName("ANCHO_PX");

            //Non pesistent properties
            Ignore(n => n.Imagen);
            Ignore(n => n.ImagenFormat);
            Ignore(n => n.SBytes);
        }
    }
}
