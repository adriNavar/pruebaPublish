using GeoSit.Data.BusinessEntities.ReclamosDiarios;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.ReclamosDiarios
{
    public class Reclamos_ReclamoDiarioMapper : EntityTypeConfiguration<Reclamos_ReclamosDiarios>
    {
        public Reclamos_ReclamoDiarioMapper()
        {
            this.ToTable("RC_RECLAMO_DIARIO");

            this.Property(a => a.Id_Reclamo)
                .HasColumnName("ID_RECLAMO");
            this.Property(a => a.Nro_Reclamo)
                .HasColumnName("NRO_RECLAMO");
            this.Property(a => a.Distrito)
                .HasColumnName("DISTRITO")
                .IsOptional();
            this.Property(a => a.Calle)
                .HasColumnName("CALLE");
            this.Property(a => a.Altura)
                .HasColumnName("ALTURA")
                .IsOptional();
            this.Property(a => a.Interseccion)
                .HasColumnName("CALLE2");
            this.Property(a => a.Manzana)
                .HasColumnName("MANZANA");
            this.Property(a => a.Fecha_Ingreso)
                .HasColumnName("FECHA_INGRESO")
                .IsOptional();
            this.Property(a => a.Id_Clase)
                .HasColumnName("ID_CLASE");
            this.Property(a => a.Id_Tipo)
                .HasColumnName("ID_TIPO");
            this.Property(a => a.Id_Motivo)
                .HasColumnName("ID_MOTIVO");
            this.Property(a => a.FechaAlta)
                .HasColumnName("FECHA_ALTA");    

            this.HasKey(a => a.Id_Reclamo);
        }
    }
}
