using GeoSit.Data.BusinessEntities.Seguridad;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class DistritosMapper : EntityTypeConfiguration<Distritos>
    {
        public DistritosMapper()
        {
            this.ToTable("CT_DISTRITO");

            this.Property(a => a.Id_Distrito)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_DISTRITO");
            this.Ignore(a => a.Geometry);
            //this.Property(a => a.Geometry)
            //    .IsRequired()
            //    .HasColumnName("GEOMETRY");
            this.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
            this.Property(a => a.Abrev)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ABREV");
            this.Property(a => a.Id_Region)
                .IsRequired()
                .HasColumnName("ID_REGION");
            this.Property(a => a.Prestacion)
                .HasColumnName("PRESTACION");
            this.Property(a => a.Id_Provincia)
                .HasColumnName("ID_PROVINCIA");
            this.Property(a => a.Apic_Gid)
                .HasColumnName("APIC_GID");
            this.Property(a => a.Apic_Id)
                .HasColumnName("APIC_ID");

            this.Property(a => a.Id_Region)
                .IsRequired()
                .HasColumnName("ID_REGION");
         

            this.HasKey(a => a.Id_Distrito);

            HasRequired(a => a.Region)
              .WithMany(c => c.Distritos)
              .HasForeignKey(a => a.Id_Region);

        }
    }
}
