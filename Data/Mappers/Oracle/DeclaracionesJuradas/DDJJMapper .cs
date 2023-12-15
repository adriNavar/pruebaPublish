using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class DDJJMapper : EntityTypeConfiguration<DDJJ>
    {
        public DDJJMapper()
        {
            this.ToTable("VAL_DDJJ");

            this.HasKey(a => a.IdDeclaracionJurada);

            this.Property(a => a.IdDeclaracionJurada)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DDJJ");

            this.Property(a => a.IdVersion)
                .IsRequired()
                .HasColumnName("ID_DDJJ_VERSION");

            this.Property(a => a.IdOrigen)
                .HasColumnName("ID_DDJJ_ORIGEN");

            this.Property(a => a.IdUnidadTributaria)
                .HasColumnName("ID_UNIDAD_TRIBUTARIA");

            this.Property(a => a.IdPoligono)
                .HasColumnName("ID_PLANO");

            this.Property(a => a.IdTramiteObjeto)
                .HasColumnName("ID_TRAMITE_OBJETO");

            this.Property(a => a.FechaVigencia)
                .HasColumnName("FECHA_VIGENCIA");
                    
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

            this.HasRequired(a => a.Version)
                .WithMany()
                .HasForeignKey(a => a.IdVersion);

            this.HasRequired(a => a.Origen)
                .WithMany()
                .HasForeignKey(a => a.IdOrigen);
        }
    }
}
