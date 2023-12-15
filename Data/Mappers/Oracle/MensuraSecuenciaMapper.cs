using GeoSit.Data.BusinessEntities.Inmuebles;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class MensuraSecuenciaMapper : EntityTypeConfiguration<MensuraSecuencia>
    {
        public MensuraSecuenciaMapper()
        {
            ToTable("INM_MENSURA_SECUENCIA");

            Property(ps => ps.Departamento)
                .HasColumnName("ID_DEPARTAMENTO")
                .IsRequired();
            
            Property(ps => ps.LetraMensura)
                .HasColumnName("LETRA_MENSURA")
                .IsRequired();
            
            Property(ps => ps.Valor)
                .HasColumnName("ULTIMO_VALOR")
                .IsRequired();

            this.HasKey(ps => new { ps.Departamento, ps.LetraMensura });

        }
    }
}
