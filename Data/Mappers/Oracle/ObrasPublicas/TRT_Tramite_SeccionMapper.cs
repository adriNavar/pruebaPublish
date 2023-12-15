using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Data.Mappers.Oracle.ObrasPublicas
{
    public class TRT_Tramite_SeccionMapper : EntityTypeConfiguration<TramiteSeccion>
    {
        public TRT_Tramite_SeccionMapper()
        {
            this.ToTable("TRT_TRAMITE_SECCION");

            this.Property(a => a.Id_Tramite_Seccion)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_TRAMITE_SECCION");
            this.Property(a => a.Id_Tramite)
                .IsRequired()
                .HasColumnName("ID_TRAMITE");
            this.Property(a => a.Id_Tipo_Seccion)
                .IsOptional()
                .HasColumnName("ID_TIPO_SECCION");
            this.Property(a => a.Detalle)        
                .HasColumnName("DETALLE");
            this.Property(a => a.Imprime)                
                .HasColumnName("IMPRIME");
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

            this.HasKey(a => a.Id_Tramite_Seccion);

            this.HasRequired(a => a.TipoSeccion)
                .WithMany()
                .HasForeignKey(a => a.Id_Tipo_Seccion);

            this.HasRequired(a => a.Tramite)
                .WithMany(c => c.Secciones)
                .HasForeignKey(a => a.Id_Tramite);
        }




    }
}
