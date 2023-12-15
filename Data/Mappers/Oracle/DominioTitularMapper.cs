using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.Mappers.Oracle
{
    public class DominioTitularMapper: EntityTypeConfiguration<DominioTitular>
    {
        public DominioTitularMapper()
        {
            ToTable("INM_DOMINIO_TITULAR");

            HasKey(dt => dt.IdDominioTitular);

            Property(dt => dt.IdDominioTitular)
                .HasColumnName("ID_DOMINIO_TITULAR")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            Property(dt => dt.DominioID)
                .HasColumnName("ID_DOMINIO")
                .IsRequired();

            Property(dt => dt.PersonaID)
                .HasColumnName("ID_PERSONA")
                .IsRequired();

            Property(dt => dt.TipoPersonaID)
                .HasColumnName("ID_TIPO_PERSONA");

            Property(dt => dt.TipoTitularidadID)
                .HasColumnName("ID_TIPO_TITULARIDAD")
                .IsRequired();

            Property(dt => dt.PorcientoCopropiedad)
                .HasColumnName("PORCIENTO_COPROPIEDAD")
                .IsRequired();

            Property(dt => dt.UsuarioAltaID)
                .HasColumnName("ID_USU_ALTA")
                .IsRequired();

            Property(dt => dt.FechaAlta)
                .HasColumnName("FECHA_ALTA")
                .IsRequired();

            Property(dt => dt.UsuarioModificacionID)
                .HasColumnName("ID_USU_MODIF")
                .IsRequired();

            Property(dt => dt.FechaModificacion)
                .HasColumnName("FECHA_MODIF")
                .IsRequired();

            Property(dt => dt.UsuarioBajaID)
                .HasColumnName("ID_USU_BAJA")
                .IsOptional();

            Property(dt => dt.FechaBaja)
                .HasColumnName("FECHA_BAJA")
                .IsOptional();

            /*this.Ignore(dt => dt.Persona);
            this.Ignore(dt => dt.Dominio);
            this.Ignore(dt => dt.TipoPersona);
            this.Ignore(dt => dt.Titular);
            this.Ignore(dt => dt.Persona);*/

            //Relaciones

            /*HasRequired(dt => dt.Dominio)
                .WithMany()
                .HasForeignKey(dt => dt.DominioID);*/

            HasRequired(dt => dt.Dominio)
                .WithMany(d => d.Titulares)
                .HasForeignKey(dt => dt.DominioID);

            HasRequired(dt => dt.Persona)
                .WithMany()
                .HasForeignKey(dt => dt.PersonaID);

            HasRequired(dt => dt.TipoTitularidad)
                .WithMany()
                .HasForeignKey(dt => dt.TipoTitularidadID);
        }
    }
}
