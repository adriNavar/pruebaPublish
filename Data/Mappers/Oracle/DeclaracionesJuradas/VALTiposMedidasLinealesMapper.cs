using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class VALTiposMedidasLinealesMapper : EntityTypeConfiguration<VALTiposMedidasLineales>
    {
        public VALTiposMedidasLinealesMapper()
        {
            this.ToTable("VAL_TIPOS_MEDIDAS_LINEALES");

            this.HasKey(a => a.IdTipoMedidaLineal);

            this.Property(a => a.IdTipoMedidaLineal)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_TIPO_MED_LIN");

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
