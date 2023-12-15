using GeoSit.Data.BusinessEntities.Seguridad;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class RegionesMapper : EntityTypeConfiguration<Regiones>
    {
        public RegionesMapper()
        {
            this.ToTable("CT_REGION");

            this.Property(a => a.Id_Region)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_REGION");
            this.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
            this.Ignore(a => a.Geometry);
            //this.Property(a => a.Geometry)
            //.IsRequired()
            //.HasMaxLength(50)
            //.IsUnicode(false)
            //.HasColumnName("GEOMETRY");
            this.Property(a => a.Id_Concesion)
               .IsRequired()
               .HasColumnName("ID_CONCESION");
            this.Property(a => a.Apic_Gid)
                .HasColumnName("APIC_GID");
            this.Property(a => a.Apic_Id)
                .HasColumnName("APIC_ID");


            this.HasKey(a => a.Id_Region);

        }
    }
}
