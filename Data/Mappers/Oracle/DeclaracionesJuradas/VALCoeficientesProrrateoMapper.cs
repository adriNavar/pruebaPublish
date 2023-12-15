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
    public class VALCoeficientesProrrateoMapper : EntityTypeConfiguration<VALCoeficientesProrrateo>
    {
        public VALCoeficientesProrrateoMapper()
        {
            this.ToTable("VAL_COEFICIENTES_PRORRATEO");

            this.HasKey(a => a.IdCoefProrrateo);

            this.Property(a => a.IdCoefProrrateo)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_COEF_PRORRATEO");

            this.Property(a => a.Piso)
                .IsRequired()
                .HasColumnName("PISO");

            this.Property(a => a.Coeficiente)
                 .IsRequired()
                 .HasColumnName("COEFICIENTE");

            this.Property(a => a.FechaVigencia)
                .HasColumnName("FECHA_VIGENCIA");

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