using GeoSit.Data.BusinessEntities.MapasTematicos;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class PredefinidoMapper : EntityTypeConfiguration<Predefinido>
    {
        public PredefinidoMapper()
        {

            this.ToTable("MT_PREDEFINIDO");

            this.Property(a => a.IdPredefinido)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_PREDEFINIDO");
            this.Property(a => a.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DESCRIPCION");
            this.Property(a => a.ConfiguracionId)
                .IsOptional()
                .HasColumnName("ID_CONFIG");
            this.Property(a => a.NombreColeccion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NOMBRE_COLECCION");
            this.Property(a => a.LeyendaConfig)
                .IsRequired()
                .HasColumnName("LEYENDA_CONFIG");
            this.Property(a => a.BorraRepetidos)
                .IsRequired()
                .HasColumnName("BORRA_REPETIDOS");
            this.Property(a => a.IdPlantillaCategoria)
                .IsRequired()
                .HasColumnName("ID_PLANTILLA_CATEGORIA");

            this.HasKey(a => a.IdPredefinido);

        }
    }
}
