using GeoSit.Data.BusinessEntities.MesaEntradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class METramiteMapper : EntityTypeConfiguration<METramite>
    {
        public METramiteMapper()
        {
            this.ToTable("ME_TRAMITE");

            this.HasKey(a => a.IdTramite);

            this.Property(a => a.IdTramite)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_TRAMITE");

            this.Property(a => a.Numero)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("NUMERO");

            this.Property(a => a.IdPrioridad)
                .IsRequired()
                .HasColumnName("ID_PRIORIDAD");

            this.Property(a => a.FechaInicio)
                .IsRequired()
                .HasColumnName("FECHA_INICIO");

            this.Property(a => a.FechaIngreso)
                .HasColumnName("FECHA_INGRESO");

            this.Property(a => a.FechaLibro)
                .HasColumnName("FECHA_LIBRO");

            this.Property(a => a.FechaVenc)
                .HasColumnName("FECHA_VENC");

            this.Property(a => a.IdJurisdiccion)
                .IsRequired()
                .HasColumnName("ID_JURISDICCION");

            this.Property(a => a.IdLocalidad)
                .HasColumnName("ID_LOCALIDAD");

            this.Property(a => a.IdTipoTramite)
                .IsRequired()
                .HasColumnName("ID_TIPO_TRAMITE");

            this.Property(a => a.IdObjetoTramite)
                .IsRequired()
                .HasColumnName("ID_OBJETO_TRAMITE");

            this.Property(a => a.Motivo)
                .IsRequired()
                .HasMaxLength(4000)
                .IsUnicode(false)
                .HasColumnName("MOTIVO");

            this.Property(a => a.IdEstado)
                .IsRequired()
                .HasColumnName("ID_ESTADO");

            this.Property(a => a.IdIniciador)
                .HasColumnName("ID_INICIADOR");

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

            this.Property(a => a.IdUnidadTributaria)
               .HasColumnName("ID_UNIDAD_TRIBUTARIA");

            this.HasRequired(a => a.Prioridad)
                .WithMany()
                .HasForeignKey(a => a.IdPrioridad);

            this.HasRequired(a => a.Tipo)
                .WithMany()
                .HasForeignKey(a => a.IdTipoTramite);

            this.HasRequired(a => a.Objeto)
                .WithMany()
                .HasForeignKey(a => a.IdObjetoTramite);

            this.HasRequired(a => a.Estado)
                .WithMany()
                .HasForeignKey(a => a.IdEstado);

            this.HasRequired(a => a.Jurisdiccion)
                .WithMany()
                .HasForeignKey(a => a.IdJurisdiccion);

            this.HasOptional(a => a.Localidad)
                .WithMany()
                .HasForeignKey(a => a.IdLocalidad);

            this.HasOptional(a => a.Iniciador)
                .WithMany()
                .HasForeignKey(a => a.IdIniciador);

            this.HasOptional(a => a.UnidadTributaria)
                .WithMany()
                .HasForeignKey(a => a.IdUnidadTributaria);

        }
    }
}
