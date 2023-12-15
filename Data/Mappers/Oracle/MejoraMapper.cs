using GeoSit.Data.BusinessEntities.Valuaciones;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class MejoraMapper : EntityTypeConfiguration<Mejora>
    {
        public MejoraMapper()
        {

            this.ToTable("INM_MEJORAS_M");

            this.Property(v => v.MejoraID)
                .HasColumnName("ID_MEJORA")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            this.Property(v => v.UnidadTributariaID)
                .HasColumnName("ID_UNIDAD_TRIBUTARIA");

            this.Property(v => v.ParcelaID)
                .HasColumnName("ID_PARCELA")
                .IsRequired();

            this.Property(v => v.SubCategoriaID)
                .HasColumnName("ID_SUBCATEGORIA")
                .IsRequired();

            this.Property(v => v.Parametro2)
                .HasColumnName("ID_ESTADO_CONSERVACION")
                .IsOptional();

            this.Property(v => v.UnidadMedida)
                .HasColumnName("UNIDAD_MEDIDA")
                .IsOptional();

            this.Property(v => v.Parametro1)
                .HasColumnName("ID_TIPO_MEJORA")
                .IsOptional();

            this.Property(v => v.Medida)
                .HasColumnName("MEDIDA")
                .IsOptional();

            this.Property(v => v.Anio)
                .HasColumnName("ANIO")
                .IsOptional();
            this.Property(v => v.MedidaSemiCubierta)
             .HasColumnName("MEDIDA_SEMICUBIERTA")
             .IsOptional();



            this.HasKey(v => v.MejoraID);

            //this.HasRequired(v => v.UnidadTributaria)
            //    .WithMany(ut => ut.mejora)
            //    .HasForeignKey(prop => prop.UnidadTributariaID);
            //  this.HasRequired(v => v.Parcela)
            //    .WithMany(p => p.mejora)
            //    .HasForeignKey(prop => prop.ParcelaID);
        }
    }
}
