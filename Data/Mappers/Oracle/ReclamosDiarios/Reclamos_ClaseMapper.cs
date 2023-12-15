using GeoSit.Data.BusinessEntities.ReclamosDiarios;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.ReclamosDiarios
{
    public class Reclamos_ClaseMapper : EntityTypeConfiguration<Reclamos_Clase>
    {
        public Reclamos_ClaseMapper()
        {
            this.ToTable("RC_CLASE");

            this.Property(a => a.Clase_Id)
                .HasColumnName("ID_CLASE")
                .IsRequired();
            this.Property(a => a.Descripcion)
                .HasColumnName("DESCRIPCION")
                .IsRequired()
                .HasMaxLength(50);
            this.Property(a => a.Habilitado)
                .HasColumnName("HABILITADO")
                .IsRequired();
            this.Property(a => a.Id)
                .HasColumnName("ID_SAR")
                .IsRequired();

            this.HasKey(a => a.Clase_Id);
        }
    }
}
