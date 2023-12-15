using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.Mappers.Oracle
{
    public class PlanoExpedienteObraMapper : EntityTypeConfiguration<PlanoExpedienteObra>
    {
        public PlanoExpedienteObraMapper()
        {
            this.ToTable("OP_EO_PLANO");
            this.Property(a => a.PlanoExpedienteObraID)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_EO_PLANO");
            this.Property(a => a.PlanoInmuebleID)
                .IsRequired()
                .HasColumnName("ID_PLANO");
            this.Property(a => a.ExpedienteObraID)
                .IsRequired()
                .HasColumnName("ID_EXPEDIENTE");

            this.HasKey(a => a.PlanoExpedienteObraID);
        }
    }
}