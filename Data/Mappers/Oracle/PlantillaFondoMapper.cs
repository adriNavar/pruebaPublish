using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.Mappers.Oracle
{
    public class PlantillaFondoMapper : EntityTypeConfiguration<PlantillaFondo>
    {
        public PlantillaFondoMapper()
        {
            //Table mapping
            ToTable("MP_PLANTILLA_FONDO");

            //Primary key
            HasKey(pf => pf.IdPlantillaFondo);

            //Properties
            Property(pf => pf.IdPlantillaFondo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_PLANTILLA_FONDO");

            Property(pf => pf.IdPlantilla)
                .IsRequired()
                .HasColumnName("ID_PLANTILLA");

            Property(pf => pf.IdResolucion)
                .IsRequired()
                .HasColumnName("ID_RESOLUCION");

            Property(pf => pf.Alto_Px)
                .IsRequired()
                .HasColumnName("ALTO_PX");

            Property(pf => pf.Ancho_Px)
                .IsRequired()
                .HasColumnName("ANCHO_PX");

            Property(pf => pf.ImagenNombre)
                .IsRequired()
                .HasColumnName("IMAGEN_NOMBRE");

            Property(pf => pf.IBytes)
                .IsRequired()
                .HasColumnName("IMAGEN");

            //Property(pf => pf.ITipo)
            //    .IsRequired()
            //    .HasColumnName("IMAGEN_TIPO");

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

            //Non persistent fields
            Ignore(pf => pf.ImagenImage);
            //Ignore(pf => pf.ImagenFormat);
            Ignore(pf => pf.PDFMemoryStream);

            //Relationship with plantilla
            HasRequired(pf => pf.Plantilla)
                .WithMany(p => p.PlantillaFondos)
                .HasForeignKey(pf => pf.IdPlantilla);

            //Relationship with Resolucion
            HasRequired(pf => pf.Resolucion)
                .WithMany(r => r.PlantillaFondos)
                .HasForeignKey(pf => pf.IdResolucion);
        }
    }
}