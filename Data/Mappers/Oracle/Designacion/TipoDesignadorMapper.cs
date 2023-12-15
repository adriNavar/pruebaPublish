using GeoSit.Data.BusinessEntities.Designaciones;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class TipoDesignadorMapper : EntityTypeConfiguration<TipoDesignador>
    {
        public TipoDesignadorMapper()
        {
            this.ToTable("INM_TIPO_DESIGNADOR")
                .HasKey(a => a.IdTipoDesignador);

            this.Property(a => a.IdTipoDesignador)
                .IsRequired()
                .HasColumnName("ID_TIPO_DESIGNADOR");

            this.Property(a => a.Nombre)
                .IsRequired()
                .HasColumnName("DESCRIPCION");

            this.Property(a => a.UsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");
        }
    }
}
