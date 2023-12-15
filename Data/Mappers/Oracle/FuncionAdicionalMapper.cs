using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.Mappers.Oracle
{
    public class FuncionAdicionalMapper : EntityTypeConfiguration<FuncionAdicional>
    {
        public FuncionAdicionalMapper()
        {
            //Table mapping
            ToTable("MP_FUNCION_ADICIONAL");

            //Primary key
            HasKey(h => h.IdFuncionAdicional);

            //Properties
            Property(h => h.IdFuncionAdicional)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_FUNCION_ADICIONAL");

            Property(h => h.Nombre)
                .IsRequired()
                .HasColumnName("NOMBRE");


        }
    }
}
