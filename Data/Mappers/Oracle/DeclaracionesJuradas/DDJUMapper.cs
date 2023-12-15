using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class DDJJUMapper : EntityTypeConfiguration<DDJJU>
    {
        public DDJJUMapper()
        {
            this.ToTable("VAL_DDJJ_U");

            this.HasKey(a => a.IdU);

            this.Property(a => a.IdU)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DDJJ_U");

            this.Property(a => a.IdDeclaracionJurada)
               .IsRequired()
               .HasColumnName("ID_DDJJ");

            this.Property(a => a.SuperficiePlano)
            .HasColumnName("SUP_PLANO");

            this.Property(a => a.SuperficieTitulo)
            .HasColumnName("SUP_TITULO");

            this.Property(a => a.AguaCorriente)
            .HasColumnName("AGUA_CTE");

            this.Property(a => a.Cloaca)
            .HasColumnName("CLOACA");

            this.Property(a => a.NumeroHabitantes)
            .HasColumnName("NRO_HABITANTES");

            this.Property(a => a.Croquis)
            .HasColumnName("CROQUIS");

            this.Property(a => a.IdMensura)
            .HasColumnName("ID_MENSURA");

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

            this.HasRequired(a => a.DeclaracionJurada)
                .WithMany(a=> a.U)
                .HasForeignKey(a => a.IdDeclaracionJurada);

            this.HasOptional(a => a.Mensuras)
                .WithMany()
                .HasForeignKey(a => a.IdMensura);


        }
    }
}
