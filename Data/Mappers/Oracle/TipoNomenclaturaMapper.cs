using GeoSit.Data.BusinessEntities.Inmuebles;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoNomenclaturaMapper : EntityTypeConfiguration<TipoNomenclatura>
    {
        public TipoNomenclaturaMapper()
        {
            this.ToTable("INM_TIPO_NOMENCLATURA")
                .HasKey(tn => tn.TipoNomenclaturaID);

            this.Property(tn => tn.TipoNomenclaturaID)
                .HasColumnName("ID_TIPO_NOMENCLATURA")
                .IsRequired();

            this.Property(tn => tn.Descripcion)
                .HasColumnName("DESCRIPCION")
                .IsRequired()
                .HasMaxLength(30);

            this.Property(tn => tn.ExpresionRegular)
                .HasColumnName("EXPRESION_REGULAR")
                .IsOptional()
                .HasMaxLength(100);

            this.Property(tn => tn.Observaciones)
                .HasColumnName("OBSERVACIONES")
                .IsOptional();

            this.Property(tn => tn.UsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            this.Property(tn => tn.FechaBaja)
                .HasColumnName("FECHA_BAJA");
        }
    }
}
