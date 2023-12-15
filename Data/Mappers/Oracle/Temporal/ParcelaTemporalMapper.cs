using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.Mappers.Oracle.Temporal
{
    public class ParcelaTemporalMapper : TablaTemporalMapper<ParcelaTemporal>
    {
        public ParcelaTemporalMapper() 
            : base("INM_PARCELA")
        {
            HasKey(p => new { p.ParcelaID, p.IdTramite });

            Property(p => p.ParcelaID)
                .HasColumnName("ID_PARCELA")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            Property(p => p.Atributos)
                .HasColumnName("ATRIBUTOS")
                .IsOptional();

            Property(p => p.TipoParcelaID)
                .HasColumnName("ID_TIPO_PARCELA")
                .IsRequired();

            Property(p => p.IdTramite)
                .HasColumnName("ID_TRAMITE")
                .IsRequired();

            Property(p => p.ClaseParcelaID)
                .HasColumnName("ID_CLASE_PARCELA")
                .IsRequired();

            Property(p => p.EstadoParcelaID)
                .HasColumnName("ID_ESTADO_PARCELA")
                .IsRequired();

            Property(p => p.OrigenParcelaID)
                .HasColumnName("ID_ORIGEN_PARCELA")
                .IsRequired();

            Property(p => p.FechaAlta)
                .HasColumnName("FECHA_ALTA")
                .IsRequired();

            Property(p => p.FechaBaja)
                .HasColumnName("FECHA_BAJA")
                .IsOptional();

            Property(p => p.FechaModificacion)
                .HasColumnName("FECHA_MODIF")
                .IsRequired();

            Property(p => p.Superficie)
                .HasColumnName("SUPERFICIE")
                .IsRequired();

            Property(p => p.UsuarioAltaID)
                .HasColumnName("ID_USU_ALTA")
                .IsRequired();

            Property(p => p.UsuarioBajaID)
                .HasColumnName("ID_USU_BAJA")
                .IsOptional();

            Property(p => p.UsuarioModificacionID)
                .HasColumnName("ID_USU_MODIF")
                .IsRequired();

            Property(p => p.AtributoZonaID)
                .HasColumnName("ID_ATRIBUTO_ZONA")
                .IsOptional();

            Property(p => p.ExpedienteAlta)
                .HasColumnName("EXPEDIENTE_ALTA")
                .IsOptional();

            Property(p => p.FechaAltaExpediente)
                .HasColumnName("FECHA_ALTA_EXP")
                .IsOptional();

            Property(p => p.ExpedienteBaja)
                .HasColumnName("EXPEDIENTE_BAJA")
                .IsOptional();

            Property(p => p.FechaBajaExpediente)
                .HasColumnName("FECHA_BAJA_EXP")
                .IsOptional();

            Property(p => p.FeatIdDGC)
                .HasColumnName("FEATID_DGC")
                .IsOptional();

            Property(p => p.FeatIdDivision)
                .HasColumnName("FEATID_DIVISION")
                .IsOptional();

            Property(p => p.PlanoId)
                .HasColumnName("ID_PLANO")
                .IsOptional();

            HasRequired(p => p.Tramite)
                .WithMany()
                .HasForeignKey(p => p.IdTramite);
        }
    }
}
