using GeoSit.Data.BusinessEntities.Seguridad;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class CargasTecnicasMapper : EntityTypeConfiguration<CargasTecnicas>
    {

        public CargasTecnicasMapper()
        {
            this.ToTable("RC_CARGA_TECNICA");

            this.Property(a => a.Id_Carga_Tecnica)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_CARGA_TECNICA");
            this.Property(a => a.Id_Analisis)
                .IsRequired()
                .HasColumnName("ID_ANALISIS");
            this.Property(a => a.Tipo_Carga)
                .IsRequired()
                .HasColumnName("TIPO_CARGA");
            this.Property(a => a.Id_Distrito)
                .IsRequired()
                .HasColumnName("ID_DISTRITO");
            this.Property(a => a.Fecha_Desde)
                .HasColumnName("FECHA_DESDE");
            this.Property(a => a.Fecha_Hasta)
                .HasColumnName("FECHA_HASTA");
            this.Property(a => a.Usuario_Alta)
                .IsRequired()
                .HasColumnName("USUARIO_ALTA");
            this.Property(a => a.Fecha_Alta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.HasKey(a => a.Id_Carga_Tecnica);

            HasRequired(a => a.AnalisisTecnico)
              .WithMany(c => c.CargasTecnicas)
              .HasForeignKey(a => a.Id_Analisis);
        }
    }
}
