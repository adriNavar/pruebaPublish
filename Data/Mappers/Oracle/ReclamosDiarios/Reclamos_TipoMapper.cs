using GeoSit.Data.BusinessEntities.ReclamosDiarios;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.ReclamosDiarios
{
    public class Reclamos_TipoMapper : EntityTypeConfiguration<Reclamos_Tipo>
    {
        public Reclamos_TipoMapper()
        {
            this.ToTable("RC_TIPO");

            this.Property(a => a.Tipo_Id)
                .HasColumnName("ID_TIPO")
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

            this.HasKey(a => a.Tipo_Id);
        }
    }
}
