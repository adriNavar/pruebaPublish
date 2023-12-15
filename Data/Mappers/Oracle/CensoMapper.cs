using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.Mappers.Oracle
{
    public class CensoMapper : EntityTypeConfiguration<Censo>
    {
        public CensoMapper()
        {
            //Table mapping
            ToTable("CT_CENSO");

            //Primary key
            HasKey(p => p.IdCenso);

            //Properties
            Property(p => p.IdCenso)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_CENSO");

            Property(p => p.Anio)
                .IsRequired()
                .HasColumnName("ANIO");

            Property(p => p.Descripcion)
                .IsRequired()
                .HasColumnName("DESCRIPCION");

            Property(p => p.IdUsuarioAlta)
                .IsRequired()
                .HasColumnName("USUARIO_ALTA");

            Property(p => p.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(p => p.IdUsuarioModificacion)
                .IsRequired()
                .HasColumnName("USUARIO_MODIFICACION");

            Property(p => p.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIFICACION");

            Property(p => p.IdUsuarioBaja)
                .IsOptional()
                .HasColumnName("USUARIO_BAJA");

            Property(p => p.FechaBaja)
                .IsOptional()
                .HasColumnName("FECHA_BAJA");

        }
    }
}
