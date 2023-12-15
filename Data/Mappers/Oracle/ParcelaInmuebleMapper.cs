using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ParcelaInmuebleMapper : EntityTypeConfiguration<Parcela>
    {
        public ParcelaInmuebleMapper()
        {
            ToTable("INM_PARCELA");

            HasKey(p => p.ParcelaID);

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

            #region Navigation Properties Relations

            HasRequired(p => p.Clase)
                .WithMany()
                .HasForeignKey(p => p.ClaseParcelaID);

            HasRequired(p => p.Estado)
                .WithMany()
                .HasForeignKey(p => p.EstadoParcelaID);

            HasRequired(p => p.Origen)
                .WithMany()
                .HasForeignKey(p => p.OrigenParcelaID);

            HasRequired(p => p.Tipo)
                .WithMany()
                .HasForeignKey(p => p.TipoParcelaID);

            HasMany(p => p.ParcelaDocumentos)
                .WithMany();


            this.HasMany(a => a.ParcelaMensuras)
                .WithOptional()
                .HasForeignKey(a => a.IdParcela);
            #endregion

            #region Ignoradas
            Ignore(p => p.Observaciones);
            Ignore(p => p.Coordenadas);
            Ignore(p => p.Zonificacion);
            Ignore(p => p.Ubicacion);
            Ignore(p => p.SuperfecieTitulo);
            Ignore(p => p.SuperfecieMensura);
            Ignore(p => p.AfectaPh);
            Ignore(p => p.ParcelaOrigenes);
            Ignore(p => p.Domicilios);
            Ignore(p => p.ResponsablesFiscales);
            Ignore(p => p.Dominios);
            Ignore(p => p.FechaVigencia);
            Ignore(p => p.ValorTierra);
            Ignore(p => p.ValorMejora);
            Ignore(p => p.ValorInmueble);
            Ignore(p => p.PorcentajeCopropiedad);
            Ignore(p => p.EstadoDeudaServicioGenerales);
            Ignore(p => p.EstadoDeudaRentas);
            Ignore(p => p.ZonaValuacion);
            Ignore(p => p.ZonaTributaria);
            Ignore(p => p.Designaciones);
            #endregion
        }
    }
}
