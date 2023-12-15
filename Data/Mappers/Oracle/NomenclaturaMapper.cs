using GeoSit.Data.BusinessEntities.Inmuebles;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class NomenclaturaInmuebleMapper : EntityTypeConfiguration<Nomenclatura>
    {
        public NomenclaturaInmuebleMapper()
        {
            this.ToTable("INM_NOMENCLATURA");

            this.Property(n => n.Nombre)
                .HasColumnName("NOMENCLATURA")
                .IsOptional();

            this.Property(n => n.NomenclaturaID)
                .HasColumnName("ID_NOMENCLATURA")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            this.Property(n => n.ParcelaID)
                .HasColumnName("ID_PARCELA")
                .IsRequired();

            this.Property(n => n.TipoNomenclaturaID)
                .HasColumnName("ID_TIPO_NOMENCLATURA")
                .IsRequired();

            this.Property(n => n.FechaAlta)
                .HasColumnName("FECHA_ALTA")
                .IsOptional();

            this.Property(n => n.FechaBaja)
                .HasColumnName("FECHA_BAJA")
                .IsOptional();

            this.Property(n => n.FechaModificacion)
                .HasColumnName("FECHA_MODIF")
                .IsOptional();

            this.Property(n => n.UsuarioAltaID)
                .HasColumnName("ID_USU_ALTA")
                .IsOptional();

            this.Property(n => n.UsuarioBajaID)
                .HasColumnName("ID_USU_BAJA")
                .IsOptional();

            this.Property(n => n.UsuarioModificacionID)
                .HasColumnName("ID_USU_MODIF")
                .IsOptional();

            this.HasKey(n => n.NomenclaturaID);

            this.HasRequired(n => n.Tipo)
                .WithMany()
                .HasForeignKey(n => n.TipoNomenclaturaID);

            this.HasRequired(nomenc => nomenc.Parcela)
                .WithMany(par => par.Nomenclaturas)
                .HasForeignKey(p => p.ParcelaID);
        }
    }
}
