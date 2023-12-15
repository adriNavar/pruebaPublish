using GeoSit.Data.BusinessEntities.Inmuebles;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class VIREquivInmDestinosMejorasMapper : EntityTypeConfiguration<VIREquivInmDestinosMejoras>
    {
        public VIREquivInmDestinosMejorasMapper()
        {
            this.ToTable("VIR_EQUIV_INM_DESTINOS_MEJORAS", "VIR_VALUACIONES")
                .HasKey(a => a.Id);

            this.Property(p => p.Id)
                .HasColumnName("ID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            this.Property(p => p.DCGIdDestinoMejora)
                .HasColumnName("DGC_ID_DESTINO_MEJORA")
                .IsOptional();

            this.Property(p => p.DCGMejoraTipoDDJJ)
                .HasColumnName("DGC_MEJ_TIPO_DDJJ")
                .IsOptional();

            this.Property(p => p.DCGDescripcion)
                .HasColumnName("DGC_DESCRIPCION")
                .IsOptional();

            this.Property(p => p.VIRUso)
                .HasColumnName("VIR_USO")
                .IsOptional();

            this.Property(p => p.VIRIdTipo)
                .HasColumnName("VIR_ID_TIPO")
                .IsOptional();

            this.Property(p => p.VIRTipoDescripcion)
                .HasColumnName("VIR_TIPO_DESCRIP")
                .IsOptional();
        }
    }
}
