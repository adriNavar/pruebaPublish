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
    public class VALCoeficientesOtrasCarMapper : EntityTypeConfiguration<VALCoeficientesOtrasCar>
    {
        public VALCoeficientesOtrasCarMapper()
        {
            this.ToTable("VAL_COEFICIENTES_OTRAS_CAR");

            this.HasKey(a => a.IdCoefOtrasCar);

            this.Property(a => a.IdCoefOtrasCar)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_COEF_OTRAS_CAR");

            this.Property(a => a.IdOtraCar)
                .IsRequired()
                .HasColumnName("ID_OTRA_CAR");

            this.Property(a => a.IdDestinoMejora)
                 .IsRequired()
                 .HasColumnName("ID_DESTINO_MEJORA");

            this.Property(a => a.ValorMinimo)
                .HasColumnName("VALOR_MINIMO");

            this.Property(a => a.ValorMaximo)
                .HasColumnName("VALOR_MAXIMO");

            this.Property(a => a.Valor)
                .HasColumnName("VALOR");

            this.Property(a => a.IdInciso)
                .IsRequired()
                .HasColumnName("ID_INCISO");

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