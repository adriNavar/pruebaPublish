using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.DeclaracionesJuradas
{
    public class INMOtraCaracteristicaMapper : EntityTypeConfiguration<INMOtraCaracteristica>
    {
        public INMOtraCaracteristicaMapper()
        {
            this.ToTable("INM_OTRAS_CARACTERISTICAS");

            this.HasKey(a => a.IdOtraCar);

            this.Property(a => a.IdOtraCar)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_OTRA_CAR");

            this.Property(a => a.Descripcion)
               .HasColumnName("DESCRIPCION");

            this.Property(a => a.EsReal)
               .HasColumnName("ES_REAL");

            this.Property(a => a.Orden)
                .HasColumnName("ORDEN");

            this.Property(a => a.Requerido)
                .HasColumnName("REQUERIDO");

            this.Property(a => a.IdVersion)
                .HasColumnName("ID_DDJJ_VERSION");
           
            this.Property(a => a.IdUsuarioAlta)
                 .IsRequired()
                 .HasColumnName("ID_USU_ALTA");

            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.IdUsuarioModif)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            this.Property(a => a.FechaModif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.IdUsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");            
        }      

    }
}
