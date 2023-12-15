using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ImagenSatelitalMapper : EntityTypeConfiguration<ImagenSatelital>
    {
        public ImagenSatelitalMapper()
        {
            //Table mapping
            ToTable("MP_IMAGEN_SATELITAL");

            //Primary key
            HasKey(pf => pf.IdImagenSatelital);

            //Properties
            Property(pf => pf.IdImagenSatelital)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_IMAGEN_SATELITAL");

            Property(pf => pf.Nombre)
                .IsRequired()
                .HasColumnName("NOMBRE");

            Property(pf => pf.Layers)
                .IsRequired()
                .HasColumnName("WMS_LAYER");

            Property(pf => pf.URL)
                .IsRequired()
                .HasColumnName("WMS_URL");

            Property(pf => pf.SRS)
                .IsRequired()
                .HasColumnName("WMS_SRS");

            Property(pf => pf.Format)
                .IsRequired()
                .HasColumnName("WMS_FORMAT");

            Property(pf => pf.FechaImagen)
                .IsRequired()
                .HasColumnName("FECHA_IMAGEN");

            Property(pf => pf.IdUsuarioAlta)
                .IsRequired()
                .HasColumnName("USUARIO_ALTA");

            Property(pf => pf.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(pf => pf.IdUsuarioModificacion)
                .IsRequired()
                .HasColumnName("USUARIO_MODIFICACION");

            Property(pf => pf.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIFICACION");

            Property(pf => pf.IdUsuarioBaja)
                .IsOptional()
                .HasColumnName("USUARIO_BAJA");

            Property(pf => pf.FechaBaja)
                .IsOptional()
                .HasColumnName("FECHA_BAJA");

        }
    }
}