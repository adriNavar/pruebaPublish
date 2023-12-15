using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.DeclaracionesJuradas
{
    public class INMIncisoMapper : EntityTypeConfiguration<INMInciso>
    {
        public INMIncisoMapper()
        {
            this.ToTable("INM_INCISOS");

            this.HasKey(a => a.IdInciso);

            this.Property(a => a.IdInciso)
                .IsRequired()
                .HasColumnName("ID_INCISO");

            this.Property(a => a.IdVersion)
               .HasColumnName("ID_DDJJ_VERSION");

            this.Property(a => a.Descripcion)            
                .HasColumnName("DESCRIPCION");            

            this.Property(a => a.IdUsuarioAlta)
                 .IsRequired()
                 .HasColumnName("ID_USU_ALTA");

            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.IdUsuarioModif)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            this.Property(a => a.FechaModif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.IdUsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");
        }      

    }
}
