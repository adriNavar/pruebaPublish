using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class DivisionAdministrativaMapper : EntityTypeConfiguration<Division>
    {
        public DivisionAdministrativaMapper()
        {
            ToTable("OA_DIVISION")
                .HasKey(a => a.FeatId);

            Ignore(a => a.WKT);

            Property(a => a.FeatId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("FEATID");
            Property(a => a.Alias)
                .HasColumnName("ALIAS");
            Property(a => a.Codigo)
                .HasColumnName("CODIGO");
            Property(a => a.Descripcion)
                .HasColumnName("DESCRIPCION");
            Property(a => a.Nombre)
                .IsRequired()
                .HasColumnName("NOMBRE");
            Property(a => a.Nomenclatura)
                .HasColumnName("NOMENCLATURA");
            Property(a => a.ObjetoPadreId)
                .HasColumnName("ID_OBJETO_PADRE");
            Property(a => a.PlanoId)
                .HasColumnName("ID_PLANO");
            Property(a => a.TipoDivisionId)
                .HasColumnName("ID_TIPO_DIVISION");
            Property(a => a.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");
            Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");
            Property(a => a.UsuarioModificacionId)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");
            Property(a => a.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");
            Property(a => a.UsuarioBajaId)
                .HasColumnName("ID_USU_BAJA");
            Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            HasRequired(x => x.TipoDivision)
                .WithMany()
                .HasForeignKey(x => x.TipoDivisionId);

            HasOptional(x => x.Localidad)
                .WithMany()
                .HasForeignKey(x => x.ObjetoPadreId);
        }
    }
}
