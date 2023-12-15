using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.Seguridad;

namespace GeoSit.Data.Mappers.Oracle.Seguridad
{
    public class AuditoriaMapper : EntityTypeConfiguration<Auditoria>
    {
        public AuditoriaMapper()
        {
            this.ToTable("SE_AUDITORIA");

            this.Property(a => a.Id_Auditoria)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_AUDITORIA");

            this.Property(a => a.Id_Usuario)
                .IsRequired()
                .HasColumnName("ID_USUARIO");

            this.Property(a => a.Id_Evento)
                .IsRequired()
                .HasColumnName("ID_EVENTO");

            this.Property(a => a.Datos_Adicionales)
                .IsOptional()
                .HasMaxLength(255)
                .HasColumnName("DATOS_ADICIONALES");

            this.Property(a => a.Fecha)
                .IsRequired()
                .HasColumnName("FECHA");

            this.Property(a => a.Ip)
                .IsOptional()
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("IP");

            this.Property(a => a.Machine_Name)
                .IsOptional()
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("MACHINE_NAME");

            this.Property(a => a.Autorizado)
                .IsRequired()
                .HasColumnName("AUTORIZADO");

            this.Property(a => a.Objeto_Origen)
                .IsOptional()
                .HasColumnName("OBJETO_ORIGEN");

            this.Property(a => a.Objeto_Modif)
                .IsOptional()
                .HasColumnName("OBJETO_MODIF");

            this.Property(a => a.Id_Tipo_Operacion)
                .IsRequired()
                .HasColumnName("ID_TIPO_OPERACION");

            this.Property(a => a.Objeto)
                .IsOptional()
                .HasMaxLength(255)
                .HasColumnName("OBJETO");

            this.Property(a => a.Id_Objeto)
                .IsRequired()
                .HasColumnName("ID_OBJETO");

            this.Property(a => a.Id_Tramite)
                .IsOptional()
                .HasColumnName("ID_TRAMITE");

            this.Property(a => a.Cantidad)
                .IsRequired()
                .HasColumnName("CANTIDAD");

            this.HasKey(a => a.Id_Auditoria);

            this.HasOptional(po => po.tramites)
                .WithMany()
                .HasForeignKey(po => po.Id_Tramite);

        }
    }
}
