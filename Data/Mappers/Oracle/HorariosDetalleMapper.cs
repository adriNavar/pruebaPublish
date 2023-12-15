using GeoSit.Data.BusinessEntities.Seguridad;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class HorariosDetalleMapper : EntityTypeConfiguration<HorariosDetalle>
    {
        public HorariosDetalleMapper()
        {
            this.ToTable("SE_HORARIO_DETALLE");

            this.Property(a => a.Id_Horario_Detalle)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_HORARIO_DETALLE");
            this.Property(a => a.Id_Horario)
                .IsRequired()
                .HasColumnName("ID_HORARIO");
            this.Property(a => a.Dia)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("DIA");
            this.Property(a => a.Hora_Inicio)
                .IsRequired()
                .HasColumnName("HORA_INICIO");
            this.Property(a => a.Hora_Fin)
                .IsRequired()
                .IsConcurrencyToken()
                .HasColumnName("HORA_FIN");

            this.HasKey(a => a.Id_Horario_Detalle);

            HasRequired(a => a.Horario)
                .WithMany(c => c.HorariosDetalle)
                .HasForeignKey(a => a.Id_Horario);
        }
    }
}
