using GeoSit.Data.BusinessEntities.ReclamosDiarios;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.ReclamosDiarios
{
    public class Reclamos_ActualizacionMapper : EntityTypeConfiguration<Reclamos_Actualizacion>
    {
        public Reclamos_ActualizacionMapper()
        {
            this.ToTable("RC_ACTUALIZACION");

            this.Property(a => a.Fecha_Parametros)
                .HasColumnName("FECHA_PARAMETROS");
            this.Property(a => a.Fecha_Reclamos_Diarios)
                .HasColumnName("FECHA_RECLAMOS_DIARIOS");
            this.Property(a => a.Nombre)
                .HasColumnName("NOMBRE")
                .HasMaxLength(50)
                .IsRequired();

            this.HasKey(a => a.Nombre);
        }
    }
}
