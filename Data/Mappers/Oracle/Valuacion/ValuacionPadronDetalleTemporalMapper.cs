using GeoSit.Data.BusinessEntities.Valuaciones;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ValuacionPadronDetalleTemporalMapper : EntityTypeConfiguration<ValuacionPadronDetalleTemporal>
    {
        public ValuacionPadronDetalleTemporalMapper()
        {

            this.ToTable("VAL_PADRON_INM_TEMP");

            this.Property(a => a.IdPadronDetalle)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_PADRON_INM");
            this.Property(a => a.IdPadron)
                .IsRequired()
                .HasColumnName("ID_PADRON");
            this.Property(a => a.IdParcela)
                .IsRequired()
                .HasColumnName("ID_PARCELA");
            this.Property(a => a.IdUnidadTributaria)
                .IsRequired()
                .HasColumnName("ID_UNIDAD_TRIBUTARIA");
            this.Property(a => a.IdAtributoZona)
                
                .HasColumnName("ID_ATRIBUTO_ZONA");
            this.Property(a => a.TipoParcela)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("TIPO_PARCELA");
            this.Property(a => a.PartidaProvincial)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PARTIDA_PROVINCIAL");
            this.Property(a => a.PartidaMunicipal)
               .HasMaxLength(20)
               .IsUnicode(false)
               .HasColumnName("PARTIDA_MUNICIPAL");
            this.Property(a => a.DomicilioInmueble)
               .HasMaxLength(500)
               .IsUnicode(false)
               .HasColumnName("DOMICILIO_INMUEBLE");
            this.Property(a => a.DomicilioFiscal)
               .HasMaxLength(500)
               .IsUnicode(false)
               .HasColumnName("DOMICILIO_FISCAL");
            this.Property(a => a.PorcentajeCodominio)
                .HasColumnName("PORCENTAJE_CODOMINIO");
            this.Property(a => a.Titular)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("TITULAR");
            this.Property(a => a.ResponsableFiscal)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("RESP_FISCAL");
            this.Property(a => a.SuperficieTierra)
               .HasColumnName("SUPERFICIE_TIERRA");
            this.Property(a => a.SuperificeCubierta)
               .HasColumnName("SUPERFICIE_CUBIERTA");
            this.Property(a => a.SuperficieSemiCubierta)
               .HasColumnName("SUPERFICIE_SEMICUBIERTA");
            this.Property(a => a.Uso)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("USO");
            this.Property(a => a.anioConstruccion)
                .HasColumnName("ANIO_CONSTRUCCION");
            this.Property(a => a.ValorTierra)
               .HasColumnName("VALOR_TIERRA");
            this.Property(a => a.ValorMejora)
               .HasColumnName("VALOR_MEJORA");
            this.Property(a => a.ValorTotal)
               .HasColumnName("VALOR_TOTAL");
            this.Property(a => a.Usuario_Alta)
                .HasColumnName("USUARIO_ALTA");
            this.Property(a => a.Fecha_Alta)
                .IsRequired()
                .IsConcurrencyToken()
                .HasColumnName("FECHA_ALTA");
            this.Property(a => a.Usuario_Modificacion)
                .HasColumnName("USUARIO_MOD");
            this.Property(a => a.Fecha_Modificacion)
                .IsConcurrencyToken()
                .HasColumnName("FECHA_MOD");
            this.Property(a => a.Usuario_Baja)
                .HasColumnName("USUARIO_BAJA");
            this.Property(a => a.Fecha_Baja)
                .IsConcurrencyToken()
                .HasColumnName("FECHA_BAJA");
            this.Property(a => a.ValorTierraPropiedad)
                     .HasColumnName("VALOR_TIERRA_PROP");
            this.Property(a => a.ValorMejoraPropiedad)
             .HasColumnName("VALOR_MEJORAS_PROP");
           

            this.HasKey(a => a.IdPadronDetalle);
        }
    }
}
