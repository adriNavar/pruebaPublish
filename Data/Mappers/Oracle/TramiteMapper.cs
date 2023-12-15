using GeoSit.Data.BusinessEntities.ObrasPublicas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TramiteMapper : EntityTypeConfiguration<Tramite>
    {
        public TramiteMapper()
        {
            this.ToTable("TRT_TRAMITE");

            this.Property(a => a.Id_Tramite)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_TRAMITE");
            this.Property(a => a.Id_Tipo_Tramite)
                .IsRequired()
                .HasColumnName("ID_TIPO_TRAMITE");
            this.Property(a => a.Fecha)
               .IsRequired()
               .HasColumnName("FECHA");
            this.Property(a => a.Nro_Tramite)
                .IsRequired()
                .HasColumnName("NRO_TRAMITE");
            this.Property(a => a.Cod_Tramite)
                .HasMaxLength(20)
                .HasColumnName("COD_TRAMITE");
            this.Property(a => a.Informe_Final)
                .HasColumnName("INFORME_FINAL");
            this.Property(a => a.Id_Usu_Alta)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");
            this.Property(a => a.Fecha_Alta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");
            this.Property(a => a.Id_Usu_Modif)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");
            this.Property(a => a.Fecha_Modif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");
            this.Property(a => a.Id_Usu_Baja)
                .HasColumnName("ID_USU_BAJA");
            this.Property(a => a.Fecha_Baja)
                .HasColumnName("FECHA_BAJA");
            this.Property(a => a.Imprime_Cab)
                .HasColumnName("IMPRIME_CAB");
            this.Property(a => a.Imprime_Per)
                .HasColumnName("IMPRIME_PER");
            this.Property(a => a.Imprime_Doc)
                .HasColumnName("IMPRIME_DOC");
            this.Property(a => a.Imprime_Final)
                .HasColumnName("IMPRIME_FINAL");
            this.Property(a => a.Imprime_UTS)
                .HasColumnName("IMPRIME_UTS");
            this.Property(a => a.Estado)
                .HasMaxLength(1)
                .HasColumnName("ESTADO");

            this.HasKey(a => a.Id_Tramite);
        }
    }
}
