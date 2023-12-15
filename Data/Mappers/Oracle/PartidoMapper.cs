using GeoSit.Data.BusinessEntities.ModuloPloteo;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class PartidoMapper : EntityTypeConfiguration<Partido>
    {
        public PartidoMapper()
        {
            this.ToTable("CT_PARTIDO");

            this.HasKey(a => a.IdPartido);

            this.Property(a => a.IdPartido)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_PARTIDO");
            //this.Ignore(a => a.Geometry);
            ////this.Property(a => a.Geometry)
            ////    .IsRequired()
            ////    .HasColumnName("GEOMETRY");
            this.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
            this.Property(a => a.IdProvincia)
                .HasColumnName("ID_PROVINCIA");
            this.Property(a => a.ApicGId)
                .HasColumnName("APIC_GID");
            this.Property(a => a.ApicId)
                .HasColumnName("APIC_ID");
            this.Property(a => a.Prestac)
                .HasColumnName("PRESTAC");
            this.Property(a => a.Abrev)
                .HasColumnName("ABREV");

            //HasRequired(a => a.Region)
            //  .WithMany(c => c.Distritos)
            //  .HasForeignKey(a => a.Id_Region);

        }
    }
}
