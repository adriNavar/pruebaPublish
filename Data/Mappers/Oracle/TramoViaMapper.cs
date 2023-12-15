using GeoSit.Data.BusinessEntities.Via;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TramoViaMapper : EntityTypeConfiguration<TramoVia>
    {
        public TramoViaMapper()
        {
            this.ToTable("GRF_TRAMO_VIA")
                .HasKey(a => a.TramoViaId);

            this.Property(a => a.TramoViaId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_TRAMO_VIA");

            this.Property(a => a.ViaId)
                .IsRequired()
                .HasColumnName("ID_VIA");
            
            this.Property(a => a.AlturaDesde)
                .HasColumnName("ALTURA_DESDE");
            
            this.Property(a => a.AlturaHasta)
                .HasColumnName("ALTURA_HASTA");
            
            this.Property(a => a.Sufijo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SUFIJO");

            this.Property(a => a.Paridad)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("PARIDAD");

            this.Property(a => a.Cpa)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("CPA");

            this.Property(a => a.Atributos)
                .HasColumnName("ATRIBUTOS");

            this.Property(a => a.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.UsuarioModifId)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            this.Property(a => a.FechaModif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.UsuarioBajaId)
                .HasColumnName("ID_USU_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            this.Property(a => a.Aforo)
                .HasColumnName("AFORO");

            this.Property(a => a.ObjetoPadreId)
                .IsRequired()
                .HasColumnName("ID_OBJETO_PADRE");
          

            this.Ignore(a => a.Geometry);

            this.HasRequired(a => a.Via)
                .WithMany(a => a.Tramos)
                .HasForeignKey(a => a.ViaId);

            this.HasRequired(a => a.Localidad)
                .WithMany()
                .HasForeignKey(a => a.ObjetoPadreId);
        }

    }
}
