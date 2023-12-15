using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.Mappers.Oracle
{
    public class PlantillaTextoMapper : EntityTypeConfiguration<PlantillaTexto>
    {
        public PlantillaTextoMapper()
        {
            //Table mapping
            ToTable("MP_PLANTILLA_TEXTO");

            //Primary key
            HasKey(pt => pt.IdPlantillaTexto);

            //Properties
            Property(pt => pt.IdPlantillaTexto)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_PLANTILLA_TEXTO");

            Property(pt => pt.IdPlantilla)
                .IsRequired()
                .HasColumnName("ID_PLANTILLA");

            Property(pt => pt.X)
                .IsRequired()
                .HasColumnName("X");

            Property(pt => pt.Y)
                .IsRequired()
                .HasColumnName("Y");

            Property(pt => pt.Tipo)
                .IsRequired()
                .HasColumnName("TIPO");

            Property(pt => pt.Origen)
                .IsOptional()
                .HasColumnName("ORIGEN");

            Property(pt => pt.FuenteTamanio)
                .IsRequired()
                .HasColumnName("FUENTE_TAMANIO");

            Property(pt => pt.FuenteNombre)
                .IsRequired()
                .HasColumnName("FUENTE_NOMBRE");

            Property(pt => pt.FuenteColor)
                .IsRequired()
                .HasColumnName("FUENTE_COLOR");

            Property(pt => pt.FuenteAlineacion)
                .IsRequired()
                .HasColumnName("FUENTE_ALINEACION");

            Property(pt => pt.FuenteEstilo)
                .IsRequired()
                .HasColumnName("FUENTE_ESTILO");

            Property(pt => pt.AtributoId)
                .IsOptional()
                .HasColumnName("ID_ATRIBUTO");

            Property(pt => pt.IdUsuarioAlta)
                .IsRequired()
                .HasColumnName("USUARIO_ALTA");

            Property(pt => pt.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(pt => pt.IdUsuarioModificacion)
                .IsRequired()
                .HasColumnName("USUARIO_MODIFICACION");

            Property(pt => pt.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIFICACION");

            Property(pt => pt.IdUsuarioBaja)
                .IsOptional()
                .HasColumnName("USUARIO_BAJA");

            Property(pt => pt.FechaBaja)
                .IsOptional()
                .HasColumnName("FECHA_BAJA");

            //Relationship with Plantilla
            HasRequired(pt => pt.Plantilla)
                .WithMany(p => p.PlantillaTextos)
                .HasForeignKey(pt => pt.IdPlantilla);

            //Relationship with Atributo
            //HasRequired(pt => pt.Atributo)
            //    .WithMany(p => p.PlantillaTextos)
            //    .HasForeignKey(pt => pt.IdAtributo);
        }
    }
}
