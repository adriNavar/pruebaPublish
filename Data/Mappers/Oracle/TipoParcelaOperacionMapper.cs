using GeoSit.Data.BusinessEntities.Inmuebles;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoParcelaOperacionMapper : EntityTypeConfiguration<TipoParcelaOperacion>
    {
        public TipoParcelaOperacionMapper()
        {
            ToTable("INM_TIPO_OPERACION");

            HasKey(tpo => tpo.Id);

            Property(tpo => tpo.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_TIPO_OPERACION")
                .IsRequired();

            Property(tpo => tpo.Descripcion)
                .HasColumnName("DESCRIPCION")
                .IsRequired();   
        }
    }
}
