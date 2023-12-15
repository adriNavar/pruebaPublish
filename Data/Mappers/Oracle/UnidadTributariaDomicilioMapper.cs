using GeoSit.Data.BusinessEntities.Inmuebles;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class UnidadTributariaDomicilioMapper : EntityTypeConfiguration<UnidadTributariaDomicilio>
    {
        public UnidadTributariaDomicilioMapper()
        {
            ToTable("INM_UT_DOMICILIO");

            HasKey(utd => new { utd.DomicilioID, utd.UnidadTributariaID });

            Property(utd => utd.DomicilioID)
                .HasColumnName("ID_DOMICILIO")
                .IsRequired();

            Property(utd => utd.UnidadTributariaID)
                .HasColumnName("ID_UNIDAD_TRIBUTARIA")
                .IsRequired();

            Property(utd => utd.TipoDomicilioID)
                .HasColumnName("ID_TIPO_DOMICILIO")
                .IsRequired();

            Property(utd => utd.UsuarioAltaID)
                .HasColumnName("ID_USU_ALTA")
                .IsOptional();

            Property(utd => utd.FechaAlta)
                .HasColumnName("FECHA_ALTA")
                .IsOptional();

            Property(utd => utd.UsuarioModificacionID)
                .HasColumnName("ID_USU_MODIF")
                .IsOptional();

            Property(utd => utd.FechaModificacion)
                .HasColumnName("FECHA_MODIF")
                .IsOptional();

            Property(utd => utd.UsuarioBajaID)
                .HasColumnName("ID_USU_BAJA")
                .IsOptional();

            Property(utd => utd.FechaBaja)
                .HasColumnName("FECHA_BAJA")
                .IsOptional();

            //Relaciones

            HasRequired(utd => utd.UnidadTributaria)
                .WithMany(ut => ut.UTDomicilios)
                .HasForeignKey(utd => utd.UnidadTributariaID);

            HasRequired(utd => utd.Domicilio)
                .WithMany(d => d.UnidadTributariaDomicilio)
                .HasForeignKey(utd => utd.DomicilioID);
        }
    }
}
