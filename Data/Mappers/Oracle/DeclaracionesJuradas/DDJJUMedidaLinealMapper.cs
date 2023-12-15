using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class DDJJUMedidaLinealMapper : EntityTypeConfiguration<DDJJUMedidaLineal>
    {
        public DDJJUMedidaLinealMapper()
        {
            this.ToTable("VAL_DDJJ_U_MED_LIN");

            this.HasKey(a => a.IdUMedidaLineal);

            this.Property(a => a.IdUMedidaLineal)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DDJJ_U_MED_LIN");

            this.Property(a => a.IdClaseParcelaMedidaLineal)
               .IsRequired()
               .HasColumnName("ID_CLA_PAR_MED_LIN");

            this.Property(a => a.IdVia)
            .HasColumnName("ID_VIA");

            this.Property(a => a.ValorMetros)
            .HasColumnName("VALOR_METROS");

            this.Property(a => a.NumeroParametro)
            .HasColumnName("NRO_PARAMETRO");

            this.Property(a => a.IdTramoVia)
            .HasColumnName("ID_TRAMO_VIA");

            this.Property(a => a.ValorAforo)
            .HasColumnName("VALOR_AFORO");

            this.Property(a => a.AlturaCalle)
            .HasColumnName("ALTURA_CALLE");

            this.Property(a => a.Calle)
            .HasColumnName("CALLE");

            this.Property(a => a.IdUFraccion)
            .HasColumnName("ID_DDJJ_U_FRACCIONES");

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

            this.HasRequired(a => a.ClaseParcelaMedidaLineal)
              .WithMany(a => a.MedidasLineales)
              .HasForeignKey(a => a.IdClaseParcelaMedidaLineal);

            this.HasRequired(a => a.Fraccion)
               .WithMany(a => a.MedidasLineales)
               .HasForeignKey(a => a.IdUFraccion);

            this.HasOptional(b => b.Tramo)
                .WithMany()
                .HasForeignKey(b => b.IdTramoVia);


        }
    }
}
