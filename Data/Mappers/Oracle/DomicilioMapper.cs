using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;

namespace GeoSit.Data.Mappers.Oracle
{
    public class DomicilioMapper : EntityTypeConfiguration<Domicilio>
    {
        public DomicilioMapper()
        {
            ToTable("INM_DOMICILIO");
            HasKey(a => a.DomicilioId);

            Property(a => a.DomicilioId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DOMICILIO");

            Property(a => a.ViaNombre)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NOMBRE_VIA");
            Property(a => a.numero_puerta)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("NUMERO_PUERTA");
            Property(a => a.piso)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PISO");
            Property(a => a.unidad)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("UNIDAD");
            Property(a => a.barrio)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BARRIO");
            Property(a => a.localidad)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("LOCALIDAD");
            Property(a => a.IdLocalidad)
                .IsOptional()
                .HasColumnName("ID_LOCALIDAD");
            Property(a => a.municipio)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MUNICIPIO");
            Property(a => a.provincia)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PROVINCIA");
            Property(a => a.ProvinciaId)
                .IsOptional()
                .HasColumnName("ID_PROVINCIA");
            Property(a => a.pais)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PAIS");
            Property(a => a.ubicacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("UBICACION");            
            Property(a => a.codigo_postal)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CODIGO_POSTAL");
            Property(a => a.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");
            Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");
            Property(a => a.UsuarioModifId)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");
            Property(a => a.FechaModif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");
            Property(a => a.UsuarioBajaId)
                .IsOptional()
                .HasColumnName("ID_USU_BAJA");
            Property(a => a.FechaBaja)
                .IsOptional()
                .HasColumnName("FECHA_BAJA");
            Property(a => a.ViaId)
                .IsOptional()
                .HasColumnName("ID_VIA");
            Property(a => a.TipoDomicilioId)
                .IsRequired()
                .HasColumnName("ID_TIPO_DOMICILIO");
            HasRequired(d => d.TipoDomicilio)
                .WithMany()
                .HasForeignKey(d => d.TipoDomicilioId);
        }
    }
}
