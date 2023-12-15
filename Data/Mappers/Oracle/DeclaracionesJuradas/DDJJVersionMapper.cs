using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class DDJJVersionMapper : EntityTypeConfiguration<DDJJVersion>
    {
        public DDJJVersionMapper()
        {
            this.ToTable("VAL_DDJJ_VERSION");

            this.HasKey(a => a.IdVersion);

            this.Property(a => a.IdVersion)
                .IsRequired()
                .HasColumnName("ID_DDJJ_VERSION");

            this.Property(a => a.TipoDeclaracionJurada)
               .IsRequired()
               .HasColumnName("TIPO_DDJJ");

            this.Property(a => a.VersionDeclaracionJurada)
                .IsRequired()
                .HasColumnName("VERSION_DDJJ");

            this.Property(a => a.Habilitado)
                .IsRequired()
                .HasColumnName("HABILITADO");

            this.Property(a => a.FechaDesde)
               .HasColumnName("FECHA_DESDE");

            this.Property(a => a.FechaHasta)
               .HasColumnName("FECHA_HASTA");

            this.Property(a => a.UsuarioAlta)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.UsuarioModif)
                .HasColumnName("ID_USU_MODIF");

            //this.Property(a => a.FechaModif)
            //    .IsRequired()
            //    .HasColumnName("FECHA_MODIF");

            this.Property(a => a.UsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

        }
    }
}
