using GeoSit.Data.BusinessEntities.ObrasParticulares;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ActaDomicilioMapper : EntityTypeConfiguration<ActaDomicilio>
    {
        public ActaDomicilioMapper()
        {
            this.ToTable("OP_ACTA_DOMICILIOS");

            this.Property(a => a.ActaID)
                .IsRequired()
                .HasColumnName("ID_ACTA");
            this.Property(a => a.id_domicilio)
                .IsRequired()
                .HasColumnName("ID_DOMICILIO");

            this.HasKey(a => new { a.ActaID, a.id_domicilio });

            this.HasRequired(a => a.domicilio).WithMany().HasForeignKey(a => a.id_domicilio);
            HasRequired(a => a.Acta)
                .WithMany()
                .HasForeignKey(a => a.ActaID);
        }
    }
}
