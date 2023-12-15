using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.Mappers.Oracle
{
    public class PersonaExpedienteObraMapper : EntityTypeConfiguration<PersonaExpedienteObra>
    {
        public PersonaExpedienteObraMapper()
        {
            ToTable("OP_EO_PERSONA");

            HasKey(pieo => new { pieo.PersonaInmuebleId, pieo.ExpedienteObraId, pieo.RolId });

            Property(pieo => pieo.PersonaInmuebleId)
                .IsRequired()
                .HasColumnName("ID_PERSONA");

            Property(pieo => pieo.ExpedienteObraId)
                .IsRequired()
                .HasColumnName("ID_EXPEDIENTE");

            Property(pieo => pieo.RolId)
                .IsRequired()
                .HasColumnName("ID_ROL");

            //Altas y bajas
            Property(pieo => pieo.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            Property(pieo => pieo.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(pieo => pieo.UsuarioModificacionId)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            Property(pieo => pieo.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            Property(pieo => pieo.UsuarioBajaId)
                .IsOptional()
                .HasColumnName("ID_USU_BAJA");

            Property(pieo => pieo.FechaBaja)
                 .IsOptional()
                 .HasColumnName("FECHA_BAJA");

            //Relaciones
            HasRequired(pieo => pieo.PersonaInmueble)
                .WithMany(p => p.PersonaInmuebleExpedienteObras)
                .HasForeignKey(pieo => pieo.PersonaInmuebleId);

            HasRequired(pieo => pieo.ExpedienteObra)
                .WithMany(eo => eo.PersonaInmuebleExpedienteObras)
                .HasForeignKey(pieo => pieo.ExpedienteObraId);

            HasRequired(pieo => pieo.Rol)
                .WithMany(r => r.PersonaInmuebleExpedienteObras)
                .HasForeignKey(pieo => pieo.RolId);
        }
    }
}
