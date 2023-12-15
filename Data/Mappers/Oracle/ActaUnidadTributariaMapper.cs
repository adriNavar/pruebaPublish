using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.Actas;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ActaUnidadTributariaMapper : EntityTypeConfiguration<ActaUnidadTributaria>
    {
        public ActaUnidadTributariaMapper()
        {
            ToTable("OP_ACTAS_UTS");

            HasKey(aut => new { aut.ActaId, aut.UnidadTributariaID });

            Property(aut => aut.ActaId)
                .IsRequired()
                .HasColumnName("ID_ACTA");

            Property(aut => aut.UnidadTributariaID)
                .IsRequired()
                .HasColumnName("ID_UNIDAD_TRIBUTARIA");

            //Relaciones
            HasRequired(aut => aut.UnidadTributaria)
                .WithMany(ut => ut.ActaUnidadTributarias)
                .HasForeignKey(aut => aut.UnidadTributariaID);

            HasRequired(aut => aut.Acta)
                .WithMany(a => a.ActaUnidadesTributarias)
                .HasForeignKey(aut => aut.ActaId);
        }
    }
}
