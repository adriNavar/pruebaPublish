using GeoSit.Data.BusinessEntities.Seguridad;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class PerfilesUsuariosMapper : EntityTypeConfiguration<PerfilesUsuarios>
    {
        public PerfilesUsuariosMapper()
        {
            

            this.Property(a => a.Id_Perfil);                
            this.Property(a => a.Id_Usuario);
            this.Property(a => a.Login);
            this.Property(a => a.Nombre);
            this.Property(a => a.Apellido);
            this.HasKey(a => a.Id_Perfil);
        }
    }
}
