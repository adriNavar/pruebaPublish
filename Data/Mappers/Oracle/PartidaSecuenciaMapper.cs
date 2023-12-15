using GeoSit.Data.BusinessEntities.Inmuebles;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class PartidaSecuenciaMapper : EntityTypeConfiguration<PartidaSecuencia>
    {
        public PartidaSecuenciaMapper()
        {
            ToTable("INM_PARTIDA_SECUENCIA");

            Property(ps => ps.Jurisdiccion)
                .HasColumnName("ID_JURISDICCION")
                .IsRequired();
            
            Property(ps => ps.TipoParcela)
                .HasColumnName("ID_TIPO_PARCELA")
                .IsRequired();
            
            Property(ps => ps.Valor)
                .HasColumnName("ULTIMO_VALOR")
                .IsRequired();

            this.HasKey(ps => new { ps.Jurisdiccion, ps.TipoParcela });

        }
    }
}
