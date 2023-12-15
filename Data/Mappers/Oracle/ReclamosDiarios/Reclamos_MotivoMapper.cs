using GeoSit.Data.BusinessEntities.ReclamosDiarios;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.ReclamosDiarios
{
    public class Reclamos_MotivoMapper : EntityTypeConfiguration<Reclamos_Motivo>
    {
        public Reclamos_MotivoMapper()
        {
            this.ToTable("RC_MOTIVO");

            this.Property(a => a.Motivo_Id)
                .HasColumnName("ID_MOTIVO")
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

            this.HasKey(a => a.Motivo_Id);
        }
    }
}
