using GeoSit.Data.BusinessEntities.ObrasParticulares;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class InspeccionMapper : EntityTypeConfiguration<Inspeccion>
    {
        public InspeccionMapper()
        {
            this.ToTable("INM_INSPECCIONES");

            this.Property(a => a.InspeccionID)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_INSPECCION");

            this.Property(a => a.InspectorID)
                .IsRequired()
                .HasColumnName("ID_INSPECTOR");

            this.Property(a => a.Descripcion)
                .IsRequired()
                .HasColumnName("DESCRIPCION")
                .IsUnicode(false);

            this.Property(a => a.FechaHoraInicio)
                .IsRequired()
                .HasColumnName("FECHA_HORA_INICIO");

            this.Property(a => a.FechaHoraFin)
                .HasColumnName("FECHA_HORA_FIN");

            this.Property(a => a.FechaHoraInicioOriginal)
               .IsRequired()
               .HasColumnName("FECHA_HORA_INICIO_ORIG");

            this.Property(a => a.FechaHoraFinOriginal)
                .HasColumnName("FECHA_HORA_FIN_ORIG");
            
            this.Property(a => a.TipoInspeccionID)
                .IsRequired()
                .HasColumnName("ID_TIPO_INSPECCION");

            this.Property(a => a.UsuarioAltaID)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.UsuarioModificacionID)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            this.Property(a => a.FechaModificacion)
                .IsRequired()
                .IsConcurrencyToken()
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.UsuarioBajaID)
                .HasColumnName("ID_USU_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            this.Property(a => a.Objeto)
                .HasColumnName("OBJETO");

            this.Property(a => a.Tipo)
                .HasColumnName("TIPO");

            this.Property(a => a.Identificador)
                .HasColumnName("IDENTIFICADOR");

            this.Property(a => a.SelectedEstado)
                .HasColumnName("ESTADO_INSPECCION");

            this.Property(a => a.FechaHoraDeInspeccion)
                .HasColumnName("FECHA_INSPECCION");

            this.Property(a => a.ResultadoInspeccion)
                .HasColumnName("OBSERVACIONES");

            //Relaciones
            this.HasRequired(i => i.Inspector)
                .WithMany(i => i.Inspecciones)
                .HasForeignKey(i => i.InspectorID);

            this.HasRequired(i => i.TipoInspeccion)
                .WithMany()
                .HasForeignKey(i => i.TipoInspeccionID);

            this.HasRequired(i => i.EstadoInspeccion)
                .WithMany()
                .HasForeignKey(i => i.SelectedEstado);
        }
    }
}
