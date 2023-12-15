using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Data.Mappers.Oracle.ObrasPublicas
{
    public class TRT_Tramite_UtsMapper : EntityTypeConfiguration<TramiteUnidadTributaria>
    {
        public TRT_Tramite_UtsMapper()
        {
            this.ToTable("TRT_TRAMITE_UTS");

            this.Property(a => a.Id_Tramite_Uts)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_TRAMITE_UTS");
            this.Property(a => a.Id_Tramite)
                .IsRequired()
                .HasColumnName("ID_TRAMITE");
            this.Property(a => a.Id_Unidad_Tributaria)
                .IsRequired()
                .HasColumnName("ID_UNIDAD_TRIBUTARIA");
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

            this.HasKey(a => a.Id_Tramite_Uts);

            this.HasRequired(p => p.UnidadTributaria)
                .WithMany()
                .HasForeignKey(p => p.Id_Unidad_Tributaria);
        }

    }
}
