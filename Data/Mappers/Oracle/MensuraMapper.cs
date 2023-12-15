using GeoSit.Data.BusinessEntities.Inmuebles;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class MensuraMapper : EntityTypeConfiguration<Mensura>
    {
        public MensuraMapper()
        {
            ToTable("INM_MENSURA");

            HasKey(p => p.IdMensura);

            Property(p => p.IdMensura)
                .HasColumnName("ID_MENSURA")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            Property(p => p.IdTipoMensura)
                .HasColumnName("ID_TIPO_MENSURA")
                .IsRequired();

            Property(p => p.IdEstadoMensura)
                .HasColumnName("ID_ESTADO_MENSURA")
                .IsRequired();

            Property(p => p.Numero)
                .HasColumnName("NUMERO");

            Property(p => p.Letra)
                .HasColumnName("LETRA");

            Property(p => p.FechaPresentacion)
                .HasColumnName("FECHA_PRESENTACION");

            Property(p => p.FechaAprobacion)
                .HasColumnName("FECHA_APROBACION");

            Property(p => p.Descripcion)
                .HasColumnName("DESCRIPCION");

            Property(p => p.MensurasRelacionadasTexto)
                .HasColumnName("MENSURAS_RELACIONADAS");

            Property(p => p.Observaciones)
                .HasColumnName("OBSERVACIONES");

            this.Property(a => a.IdUsuarioAlta)
               .IsRequired()
               .HasColumnName("ID_USU_ALTA");

            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.IdUsuarioModif)
                .HasColumnName("ID_USU_MODIF");

            this.Property(a => a.FechaModif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.IdUsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            this.HasRequired(a => a.TipoMensura)
                .WithMany()
                .HasForeignKey(a => a.IdTipoMensura);

            this.HasRequired(a => a.EstadoMensura)
                .WithMany()
                .HasForeignKey(a => a.IdEstadoMensura);

            this.Ignore(a => a.DescripcionTipoMensura);
            this.Ignore(a => a.Archivo);

        }
    }
}
