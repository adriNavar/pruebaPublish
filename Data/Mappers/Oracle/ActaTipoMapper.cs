using GeoSit.Data.BusinessEntities.Actas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ActaTipoMapper : EntityTypeConfiguration<ActaTipo>
    {
        public ActaTipoMapper()
        {
            this.ToTable("OP_ACTA_TIPO");

            this.Property(a => a.ActaTipoId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_ACTA_TIPO");
            this.Property(a => a.Descripcion)
                .IsRequired()
                .HasColumnName("DESCRIPCION");


            this.HasKey(a => a.ActaTipoId);
        }
    }
}
