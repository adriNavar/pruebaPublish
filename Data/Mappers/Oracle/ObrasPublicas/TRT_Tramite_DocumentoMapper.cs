using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Data.Mappers.Oracle.ObrasPublicas
{
    public class TRT_Tramite_DocumentoMapper : EntityTypeConfiguration<TramiteDocumento>
    {
        public TRT_Tramite_DocumentoMapper()
        {
            this.ToTable("TRT_TRAMITE_DOCUMENTO");

            this.Property(a => a.Id_Tramite_Documento)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_TRAMITE_DOCUMENTO");
            this.Property(a => a.Id_Tramite)
                .IsRequired()
                .HasColumnName("ID_TRAMITE");
            this.Property(a => a.Id_Documento)
                .IsRequired()
                .HasColumnName("ID_DOCUMENTO");
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

            this.HasKey(a => a.Id_Tramite_Documento);

        }

    }
}
