using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class DDJJDominioMapper : EntityTypeConfiguration<DDJJDominio>
    {
        public DDJJDominioMapper()
        {
            this.ToTable("VAL_DDJJ_DOMINIO");

            this.HasKey(a => a.IdDominio);

            this.Property(a => a.IdDominio)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DDJJ_DOMINIO");

            this.Property(a => a.IdDeclaracionJurada)
               .IsRequired()
               .HasColumnName("ID_DDJJ");

            this.Property(a => a.IdTipoInscripcion)
                .IsRequired()
                .HasColumnName("ID_TIPO_INSCRIPCION");

            this.Property(a => a.Inscripcion)
            .IsRequired()
            .HasColumnName("INSCRIPCION");

            this.Property(a => a.Fecha)
               .IsRequired()
               .HasColumnName("FECHA");

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

            this.HasRequired(a => a.TipoInscripcionObj)
                 .WithMany()
                 .HasForeignKey(a => a.IdTipoInscripcion);

            this.HasRequired(a => a.DeclaracionJurada)
             .WithMany(a => a.Dominios)
             .HasForeignKey(a => a.IdDeclaracionJurada);

            this.Ignore(a => a.TipoInscripcion);
        }
    }
}
