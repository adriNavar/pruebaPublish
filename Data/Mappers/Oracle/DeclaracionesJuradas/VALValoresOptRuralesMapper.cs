using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class VALValoresOptRuralesMapper : EntityTypeConfiguration<VALValoresOptRurales>
    {
        public VALValoresOptRuralesMapper()
        {
            this.ToTable("VAL_VALORES_OPT_RURALES");

            this.HasKey(a => a.IdValOptRural);

            this.Property(a => a.IdValOptRural)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_VAL_OPT_RURAL");

            this.Property(a => a.IdDepartamento)
               .IsRequired()
               .HasColumnName("ID_DEPARTAMENTO");

            this.Property(a => a.Valor)
                .IsRequired()
                .HasColumnName("VALOR");                

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
