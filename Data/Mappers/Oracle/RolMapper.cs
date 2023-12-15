using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasParticulares;

namespace GeoSit.Data.Mappers.Oracle
{
    public class RolMapper : EntityTypeConfiguration<Rol>
    {
        public RolMapper()
        {
            ToTable("OP_ROLES");

            HasKey(a => a.RolId);

            Property(a => a.RolId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_ROL");

            Property(a => a.Descripcion)
                .IsRequired()
                .HasColumnName("DESCRIPCION")
                .HasMaxLength(30)
                .IsUnicode(false);            

            HasMany(a => a.PersonaInmuebleExpedienteObras)
                .WithRequired(e => e.Rol)
                .WillCascadeOnDelete(false);
        }
    }
}
