using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class DDJJDominioTitularMapper : EntityTypeConfiguration<DDJJDominioTitular>
    {
        public DDJJDominioTitularMapper()
        {
            this.ToTable("VAL_DDJJ_DOMINIO_TITULAR");

            this.HasKey(a => a.IdDominioTitular);

            this.Property(a => a.IdDominioTitular)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DDJJ_DOMINIO_TITULAR");

            this.Property(a => a.IdPersona)
               .IsRequired()
               .HasColumnName("ID_PERSONA");

            this.Property(a => a.IdTipoTitularidad)
                .IsRequired()
                .HasColumnName("ID_TIPO_TITULARIDAD");

            this.Property(a => a.IdDominio)
            .IsRequired()
            .HasColumnName("ID_DDJJ_DOMINIO");

            this.Property(a => a.PorcientoCopropiedad)
               .IsRequired()
               .HasColumnName("PORCIENTO_COPROPIEDAD");

            this.Property(a => a.IdUsuarioAlta)
               .IsRequired()
               .HasColumnName("ID_USU_ALTA");            

            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.IdUsuarioModif)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            this.Property(a => a.FechaModif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.IdUsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            this.HasRequired(a => a.Dominio)
              .WithMany(a=> a.Titulares)
              .HasForeignKey(a => a.IdDominio);

            this.HasRequired(a => a.Persona)
              .WithMany()
              .HasForeignKey(a => a.IdPersona);

            this.Ignore(a => a.NombreCompleto);
            this.Ignore(a => a.TipoNoDocumento);
            this.Ignore(a => a.TipoTitularidad);

            this.HasRequired(a => a.TT)
              .WithMany()
              .HasForeignKey(a => a.IdTipoTitularidad);

        }
    }
}
