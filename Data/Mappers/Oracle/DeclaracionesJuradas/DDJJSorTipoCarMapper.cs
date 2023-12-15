using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class DDJJSorTipoCarMapper : EntityTypeConfiguration<DDJJSorTipoCaracteristica>
    {
        public DDJJSorTipoCarMapper()
        {
            this.ToTable("VAL_SOR_TIPO_CAR");

            this.HasKey(a => a.IdSorTipoCaracteristica);

            this.Property(a => a.IdSorTipoCaracteristica)
                .IsRequired()
                .HasColumnName("ID_SOR_TIPO_CAR");

            this.Property(a => a.Descripcion)
               .IsRequired()
               .HasColumnName("DESCRIPCION");         

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
        }
    }
}
