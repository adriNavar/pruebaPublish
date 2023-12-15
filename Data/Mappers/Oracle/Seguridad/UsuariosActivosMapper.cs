using GeoSit.Data.BusinessEntities.Seguridad;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class UsuariosActivosMapper : EntityTypeConfiguration<UsuariosActivos>
    {
        public UsuariosActivosMapper()
        {
          
            this.ToTable("SE_USUARIO_ACTIVO");

            this.Property(a => a.Id_Usuario_Activo)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_USUARIO_ACTIVO");
            this.Property(a => a.Id_Usuario)
                .HasColumnName("ID_USUARIO");
            this.Property(a => a.Fecha)
                .HasColumnName("FECHA");
            this.Property(a => a.Token)
                .HasColumnName("TOKEN");
            this.Property(a => a.Heartbeat)
                .HasColumnName("HEARTBEAT");


            this.HasKey(a => a.Id_Usuario_Activo);
        }
    }
}
