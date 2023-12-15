using GeoSit.Data.BusinessEntities.ObrasParticulares;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class InspectorMapper : EntityTypeConfiguration<Inspector>
    {
        public InspectorMapper()
        {
            this.ToTable("INM_INSPECTORES");
            this.Property(a => a.InspectorID)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_INSPECTOR");
            this.Property(a => a.EsPlanificador)
                .IsRequired()
                .HasColumnName("ES_PLANIFICADOR");
            this.Property(a => a.UsuarioID)
                .IsRequired()
                .HasColumnName("ID_USUARIO");
            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");
            this.Property(a => a.UsuarioBajaID)
                .HasColumnName("USUARIO_BAJA");
            
            this.HasKey(a => a.InspectorID);
            
            this.HasRequired(a => a.Usuario).WithMany().HasForeignKey(a=>a.UsuarioID);

            this.Ignore(a => a.TiposInspeccionSelected);
            this.Ignore(a => a.UsuarioUpdate);
        }
    }
}
