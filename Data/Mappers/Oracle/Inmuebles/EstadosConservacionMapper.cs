using GeoSit.Data.BusinessEntities.Inmuebles;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class EstadosConservacionMapper : EntityTypeConfiguration<EstadosConservacion>
    {
        public EstadosConservacionMapper()
        {

            this.ToTable("INM_ESTADO_CONSERVACION");

            this.Property(a => a.EstadoConservacionID)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_ESTADO_CONSERVACION");
            this.Property(a => a.Descripcion)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("DESCRIPCION");


            this.HasKey(a => a.EstadoConservacionID);
        }
    }
}
