using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.Mappers.Oracle
{
    public class UnidadTributariaPersonaMapper: EntityTypeConfiguration<UnidadTributariaPersona>
    {
        public UnidadTributariaPersonaMapper()
        {
            ToTable("INM_UT_PERSONA");

            HasKey(utp => new { utp.UnidadTributariaID, utp.PersonaID });

            Property(utp => utp.UnidadTributariaID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_UNIDAD_TRIBUTARIA")
                .IsRequired();

            Property(utp => utp.PersonaID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_PERSONA")
                .IsRequired();

            Property(utp => utp.TipoPersonaID)
                .HasColumnName("ID_TIPO_PERSONA")
                .IsRequired();

            Property(utp => utp.UsuarioAltaID)
                .HasColumnName("ID_USU_ALTA")
                .IsRequired();

            Property(utp => utp.FechaAlta)
                .HasColumnName("FECHA_ALTA")
                .IsRequired();

            Property(utp => utp.UsuarioModificacionID)
                .HasColumnName("ID_USU_MODIF")
                .IsRequired();

            Property(utp => utp.FechaModificacion)
                .HasColumnName("FECHA_MODIF")
                .IsRequired();

            Property(utp => utp.UsuarioBajaID)
                .HasColumnName("ID_USU_BAJA")
                .IsOptional();

            Property(utp => utp.FechaBaja)
                .HasColumnName("FECHA_BAJA")
                .IsOptional();

            Property(utp => utp.CodSistemaTributario)
                .HasColumnName("CODIGO_SISTEMA_TRIBUTARIO")
                .IsOptional();

            //Relaciones

            HasRequired(utd => utd.UnidadTributaria)
                .WithMany()
                .HasForeignKey(utd => utd.UnidadTributariaID);

            HasRequired(utd => utd.Persona)
                .WithMany()
                .HasForeignKey(utd => utd.PersonaID);

            //Ignorados
            Ignore(utp => utp.PersonaSavedId);
        }
    }
}
