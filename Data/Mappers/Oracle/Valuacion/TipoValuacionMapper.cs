using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.Valuaciones;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoValuacionMapper : EntityTypeConfiguration<TipoValuacion>
    {

        public TipoValuacionMapper()
        {
            this.ToTable("VAL_TIPO_VALUACION");

            this.Property(v => v.TipoValuacionID)
                .HasColumnName("ID_TIPO_VALUACION")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            this.Property(v => v.Descripcion)
                .HasColumnName("DESCRIPCION")
                .IsRequired();

            this.Property(v => v.IdFiltroParcela)
                .HasColumnName("ID_FILTRO_PARCELA")
                .IsOptional();

            this.Property(v => v.MetodoValuacion)
                .HasColumnName("METODO_VALUACION")
                .IsRequired();

            this.Property(v => v.Parametro1)
                .HasColumnName("PARAMETRO1")
                .IsRequired();

            this.Property(v => v.TipoParametro1)
                .HasColumnName("PARAMETRO1_TIPO")
                .IsOptional();

            this.Property(v => v.NombreParametro1)
                .HasColumnName("PARAMETRO1_NOMBRE")
                .IsOptional();
            this.Property(v => v.Parametro2)
                .HasColumnName("PARAMETRO2")
                .IsRequired();

            this.Property(v => v.TipoParametro2)
                .HasColumnName("PARAMETRO2_TIPO")
                .IsOptional();

            this.Property(v => v.NombreParametro2)
                .HasColumnName("PARAMETRO2_NOMBRE")
                .IsOptional();
            this.Property(v => v.Destino)
                .HasColumnName("DESTINO")
                .IsRequired();


            this.HasKey(v => v.TipoValuacionID);

            
        }
    }
}
