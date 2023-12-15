using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ObjetoAdministrativoMapper : EntityTypeConfiguration<Objeto>
    {
        public ObjetoAdministrativoMapper()
        {
            this.ToTable("OA_OBJETO")
                .HasKey(a => a.FeatId);

            this.Property(a => a.FeatId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("FEATID");

            this.Property(a => a.Alias)
                .HasColumnName("ALIAS");
            this.Property(a => a.Codigo)
                .HasColumnName("CODIGO");
            this.Property(a => a.Descripcion)
                .HasColumnName("DESCRIPCION");
            this.Property(a => a.Atributos)
                .HasColumnName("ATRIBUTOS");
            this.Property(a => a.Nombre)
                .HasColumnName("NOMBRE");
            this.Property(a => a.Nomenclatura)
                .HasColumnName("NOMENCLATURA");
            this.Property(a => a.ObjetoPadreId)
                .HasColumnName("ID_OBJETO_PADRE");
            this.Property(a => a.TipoObjetoId)
                .HasColumnName("ID_TIPO_OBJETO")
                .IsRequired();
            this.Property(a => a.UsuarioAlta)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");
            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");
            this.Property(a => a.UsuarioModificacion)
                .HasColumnName("ID_USU_MODIF");
            this.Property(a => a.FechaModificacion)
                .HasColumnName("FECHA_MODIF");
            this.Property(a => a.UsuarioBaja)
                .HasColumnName("ID_USU_BAJA");
            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            this.HasRequired(a => a.TipoObjeto)
                .WithMany()
                .HasForeignKey(a => a.TipoObjetoId);

            this.HasOptional(a => a.Padre)
                .WithMany()
                .HasForeignKey(a => a.ObjetoPadreId);

        }
    }
}
