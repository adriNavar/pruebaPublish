using GeoSit.Data.BusinessEntities.MesaEntradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class MEPrioridadTipoMapper : EntityTypeConfiguration<MEPrioridadTipo>
    {
        public MEPrioridadTipoMapper()
        {
            this.ToTable("ME_PRIORIDAD_TIPO")
                .HasKey(a => a.IdPrioridadTipo);

            this.Property(a => a.IdPrioridadTipo)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_PRIORIDAD_TIPO");

            this.Property(a => a.IdTipoTramite)
              .IsRequired()
              .HasColumnName("ID_TIPO_TRAMITE");

            this.Property(a => a.IdPrioridadTramite)
                .IsRequired()
                .HasColumnName("ID_PRIORIDAD_TRAMITE");

            this.Property(a => a.UsuarioAlta)
                .IsRequired()
                .HasColumnName("USUARIO_ALTA");

            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.UsuarioModif)
                .IsRequired()
                .HasColumnName("USUARIO_MODIF");

            this.Property(a => a.FechaModif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.UsuarioBaja)
                .HasColumnName("USUARIO_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            this.HasRequired(a => a.TipoTramite)
                .WithMany(t => t.PrioridadesTipos)
                .HasForeignKey(a => a.IdTipoTramite);
        }
    }
}
