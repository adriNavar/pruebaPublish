using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Data.Mappers.Oracle.ObrasPublicas
{
    public class TRT_Tipo_TramiteMapper : EntityTypeConfiguration<TipoTramite>
    {
        public TRT_Tipo_TramiteMapper()
        {
            this.ToTable("TRT_TIPO_TRAMITE");

            this.Property(a => a.Id_Tipo_Tramite)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_TIPO_TRAMITE");
            this.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(60)
                .HasColumnName("NOMBRE");
            this.Property(a => a.Autonumerico)
                .HasColumnName("AUTONUMERICO");
            this.Property(a => a.Numerador)
                .HasColumnName("NUMERADOR");
            this.Property(a => a.Imprime_Cab)
                .HasColumnName("IMPRIME_CAB");
            this.Property(a => a.Imprime_UTS)
               .HasColumnName("IMPRIME_UTS");
            this.Property(a => a.Imprime_Per)
                .HasColumnName("IMPRIME_PER");
            this.Property(a => a.Imprime_Doc)
                .HasColumnName("IMPRIME_DOC");
            this.Property(a => a.Imprime_Final)
              .HasColumnName("IMPRIME_FINAL");
            this.Property(a => a.Plantilla_Final)
                .HasColumnName("PLANTILLA_FINAL");
            this.Property(a => a.Id_Usu_Alta)
               .HasColumnName("ID_USU_ALTA");
            this.Property(a => a.Fecha_Alta)
                .HasColumnName("FECHA_ALTA");
            this.Property(a => a.Id_Usu_Modif)
                .HasColumnName("ID_USU_MODIF");
            this.Property(a => a.Fecha_Modif)
                .HasColumnName("FECHA_MODIF");
            this.Property(a => a.Id_Usu_Baja)
                .HasColumnName("ID_USU_BAJA");
            this.Property(a => a.Fecha_Baja)
                .HasColumnName("FECHA_BAJA");

            this.HasKey(a => a.Id_Tipo_Tramite);

        }

    }
}
