using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.Mappers.Oracle
{
    public class FuncAdicParametroMapper : EntityTypeConfiguration<FuncAdicParametro>
    {
        public FuncAdicParametroMapper()
        {
            //Table mapping
            ToTable("MP_FUNC_ADIC_PARAMETRO");

            //Primary key
            HasKey(f => f.IdFuncAdicParametro);

            //Properties
            Property(f => f.IdFuncAdicParametro)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_FUNC_ADIC_PARAMETRO");

            Property(f => f.IdFuncionAdicional)
                //.HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_FUNCION_ADICIONAL");

            Property(f => f.Campo)
                .IsRequired()
                .HasColumnName("CAMPO");

            Property(f => f.Valor)
                .IsRequired()
                .HasColumnName("VALOR");

            Property(f => f.Descripcion)
                .IsOptional()
                .HasColumnName("DESCRIPCION");

            //Relationship with FuncionAdicional
            HasRequired(f => f.FuncionAdicional)
                .WithMany(fa => fa.FuncAdicParametros)
                .HasForeignKey(f => f.IdFuncionAdicional);

        }
    }
}
