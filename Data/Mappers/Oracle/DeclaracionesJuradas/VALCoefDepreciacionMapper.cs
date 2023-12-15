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
    public class VALCoefDepreciacionMapper : EntityTypeConfiguration<VALCoefDepreciacion>
    {
        public VALCoefDepreciacionMapper()
        {
            this.ToTable("VAL_COEF_DEPRECIACION");

            this.HasKey(a => a.IdCoefDepreciacion);

            this.Property(a => a.IdCoefDepreciacion)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_COEF_DEPRECIACION");

            this.Property(a => a.IdInciso)
                .IsRequired()
                .HasColumnName("ID_INCISO");

            this.Property(a => a.IdEstadoConservacion)
                 .IsRequired()
                 .HasColumnName("ID_ESTADO_CONSERVACION");

            this.Property(a => a.EdadEdificacion)
                .HasColumnName("EDAD_EDIFICACION");

            this.Property(a => a.Coeficiente)
                .HasColumnName("COEFICIENTE");           

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