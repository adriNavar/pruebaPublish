using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Data.Mappers.Oracle.ObrasPublicas
{
    public class TRT_Tipo_SeccionMapper : EntityTypeConfiguration<TramiteTipoSeccion>
    {
        public TRT_Tipo_SeccionMapper()
        {
            this.ToTable("TRT_TIPO_SECCION");

            this.Property(a => a.Id_Tipo_Seccion)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_TIPO_SECCION");
            this.Property(a => a.Id_Tipo_Tramite)
                .IsRequired()
                .HasColumnName("ID_TIPO_TRAMITE");
            this.Property(a => a.Nombre )
                .IsRequired()
                .HasMaxLength(60)
                .HasColumnName("NOMBRE");
            this.Property(a => a.Plantilla)
                .HasColumnName("PLANTILLA");
            this.Property(a => a.Imprime )
                .HasColumnName("IMPRIME");
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

            this.HasKey(a => a.Id_Tipo_Seccion);

        }

    }
}

