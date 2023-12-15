using GeoSit.Data.BusinessEntities.Seguridad;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class PerfilesHistMapper : EntityTypeConfiguration<PerfilesHist>
    {
        public PerfilesHistMapper()
        {
            this.ToTable("SE_PERFIL_HIST");

            this.Property(a => a.Id_Perfil_Hist)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_PERFIL_HIST");
            this.Property(a => a.Id_Perfil)
                .IsRequired()
                .HasColumnName("ID_PERFIL");
            this.Property(a => a.Id_Horario)
                .IsRequired()
                .HasColumnName("ID_HORARIO");
            this.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");                       
            this.Property(a => a.Usuario_Operacion)
                .IsRequired()
                .HasColumnName("USUARIO_OPERACION");
            this.Property(a => a.Fecha_Operacion)
                .IsRequired()
                .IsConcurrencyToken()
                .HasColumnName("FECHA_OPERACION");
            this.HasKey(a => a.Id_Perfil_Hist);
        }
    }
}
