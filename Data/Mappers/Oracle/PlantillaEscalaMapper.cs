using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.Mappers.Oracle
{
    public class PlantillaEscalaMapper : EntityTypeConfiguration<PlantillaEscala>
    {
        public PlantillaEscalaMapper()
        {
            //Table mapping
            ToTable("MP_PLANTILLA_ESCALA");

            //Primary key
            HasKey(pe => pe.IdPlantillaEscala);

            //Properties
            Property(pe => pe.IdPlantillaEscala)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_PLANTILLA_ESCALA");

            Property(pe => pe.IdPlantilla)
                .IsRequired()
                .HasColumnName("ID_PLANTILLA");

            Property(pe => pe.Escala)
                .IsRequired()
                .HasColumnName("ESCALA");
            
            Property(pe => pe.IdUsuarioAlta)
                .IsRequired()
                .HasColumnName("USUARIO_ALTA");

            Property(pe => pe.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(pe => pe.IdUsuarioModificacion)
                .IsRequired()
                .HasColumnName("USUARIO_MODIFICACION");

            Property(pe => pe.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIFICACION");

            Property(pe => pe.IdUsuarioBaja)
                .IsOptional()
                .HasColumnName("USUARIO_BAJA");

            Property(pe => pe.FechaBaja)
                .IsOptional()
                .HasColumnName("FECHA_BAJA");

            //Relationship with plantilla
            HasRequired(pe => pe.Plantilla)
                .WithMany(p => p.PlantillaEscalas)
                .HasForeignKey(pe => pe.IdPlantilla);
        }
    }
}
