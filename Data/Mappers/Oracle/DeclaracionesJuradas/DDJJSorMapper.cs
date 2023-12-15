using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class DDJJSorMapper : EntityTypeConfiguration<DDJJSor>
    {
        public DDJJSorMapper()
        {
            this.ToTable("VAL_DDJJ_SOR");

            this.HasKey(a => a.IdSor);

            this.Property(a => a.IdSor)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DDJJ_SOR");

            this.Property(a => a.IdDeclaracionJurada)
               .IsRequired()
               .HasColumnName("ID_DDJJ");

            this.Property(a => a.IdLocalidad)
            .HasColumnName("ID_LOCALIDAD");

            this.Property(a => a.IdCamino)
            .HasColumnName("ID_CAMINO");

            this.Property(a => a.DistanciaCamino)
            .HasColumnName("DISTANCIA_CAMINO");

            this.Property(a => a.DistanciaLocalidad)
            .HasColumnName("DISTANCIA_LOCALIDAD");

            this.Property(a => a.DistanciaEmbarque)
            .HasColumnName("DISTANCIA_EMBARQUE");

            this.Property(a => a.NumeroHabitantes)
            .HasColumnName("NRO_HABITANTES");

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
              .WithMany(a=> a.Sor)
              .HasForeignKey(a => a.IdDeclaracionJurada);

            this.HasOptional(b => b.Via)
                 .WithMany()
                 .HasForeignKey(b => b.IdCamino);

            this.HasOptional(b => b.Objeto)
                 .WithMany()
                 .HasForeignKey(b => b.IdLocalidad);

            this.HasOptional(a => a.Mensuras)
                .WithMany()
                .HasForeignKey(a => a.IdMensura);

        }
    }
}
