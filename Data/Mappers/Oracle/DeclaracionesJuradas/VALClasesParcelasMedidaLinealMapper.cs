using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class VALClasesParcelasMedidaLinealMapper : EntityTypeConfiguration<VALClasesParcelasMedidaLineal>
    {
        public VALClasesParcelasMedidaLinealMapper()
        {
            this.ToTable("VAL_CLASES_PARCELAS_MED_LIN");

            this.HasKey(a => a.IdClasesParcelasMedidaLineal);

            this.Property(a => a.IdClasesParcelasMedidaLineal)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_CLA_PAR_MED_LIN");

            this.Property(a => a.IdClaseParcela)
               .IsRequired()
               .HasColumnName("ID_CLASE_PARCELA");

            this.Property(a => a.IdTipoMedidaLineal)
               .IsRequired()
               .HasColumnName("ID_TIPO_MED_LIN");

            this.Property(a => a.Orden)
                .HasColumnName("ORDEN");

            this.Property(a => a.RequiereLongitud)
                .HasColumnName("REQUIERE_LONGITUD");

            this.Property(a => a.RequiereAforo)
                .HasColumnName("REQUIERE_AFORO");

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

            this.HasRequired(a => a.ClaseParcela)
              .WithMany(a => a.ClasesParcelasMedidasLineales)
              .HasForeignKey(a => a.IdClaseParcela);

            this.HasRequired(a => a.TipoMedidaLineal)
            .WithMany(a => a.ClasesParcelasMedidasLineales)
            .HasForeignKey(a => a.IdTipoMedidaLineal);
        }
    }
}
