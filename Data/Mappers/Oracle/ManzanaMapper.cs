using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ManzanaMapper : EntityTypeConfiguration<Manzana>
    {
        public ManzanaMapper()
        {
            this.ToTable("CT_DIVISION");
            this.Property(a => a.FeatId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("FEATID");

            this.Property(a => a.Nomenclatura)
                .IsRequired()
                .HasColumnName("DIV_DESCRIPTOR");

            //this.Property(a => a.Geom)
            //    .IsRequired()
            //    .HasColumnName("GEOMETRY");

            //this.Property(a => a.WKT)
            //    .IsOptional()
            //    .HasColumnName("WKT");

            //this.Ignore(a => a.WKT);
            this.Ignore(a => a.Geom);

            //CLAVE PRIMARIA
            this.HasKey(a => a.FeatId);


        }
    }
}
