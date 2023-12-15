using GeoSit.Data.BusinessEntities.ObrasPublicas;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.Mappers.Oracle.ObrasPublicas
{
    public class TRT_Tramite_PermisosMapper : EntityTypeConfiguration<TramitePermisos>
    {
        public TRT_Tramite_PermisosMapper() 
        { 
            this.ToTable("TRT_SECCION_FUNCION");

            this.Property(a => a.ID_SECCION_FUNCION)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_SECCION_FUNCION");

            this.Property(a => a.ID_TIPO_SECCION)
                .IsRequired()
                .HasColumnName("ID_TIPO_SECCION");

            this.Property(a => a.ID_FUNCION)
                .IsRequired()
                .HasColumnName("ID_FUNCION");

            this.Property(a => a.ID_USU_ALTA)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            this.Property(a => a.FECHA_ALTA)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.ID_USU_MODIF)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            this.Property(a => a.FECHA_MODIF)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.ID_USU_BAJA)
                .HasColumnName("ID_USU_BAJA");

            this.Property(a => a.FECHA_BAJA)
                .HasColumnName("FECHA_BAJA");

            this.Property(a => a.ID_TIPO_TRAMITE)
                .HasColumnName("ID_TIPO_TRAMITE");
        }
    }
}
