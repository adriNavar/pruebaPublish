using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class DDJJUFraccionesMapper : EntityTypeConfiguration<DDJJUFracciones>
    {
        public DDJJUFraccionesMapper()
        {
            this.ToTable("VAL_DDJJ_U_FRACCIONES");

            this.HasKey(a => a.IdFraccion);

            this.Property(a => a.IdFraccion)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DDJJ_U_FRACCIONES");

            this.Property(a => a.IdU)
               .IsRequired()
               .HasColumnName("ID_DDJJ_U");

            this.Property(a => a.NumeroFraccion)
                .IsRequired()
                .HasColumnName("NRO_FRACCION");

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

            this.HasRequired(a => a.U)
              .WithMany(a=> a.Fracciones)
              .HasForeignKey(a => a.IdU);
        }
    }
}
