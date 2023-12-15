using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.Mappers.Oracle.Temporal
{
    public class DivisionTemporalMapper : TablaTemporalMapper<DivisionTemporal>
    {
        public DivisionTemporalMapper() 
            : base("OA_DIVISION")
        {
            HasKey(p => new { p.FeatId, p.IdTramite });

            Property(p => p.FeatId)
                .HasColumnName("FEATID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();
            this.Property(a => a.Alias)
                .HasColumnName("ALIAS");
            this.Property(a => a.Codigo)
                .HasColumnName("CODIGO");
            this.Property(a => a.Descripcion)
                .IsRequired()
                .HasColumnName("DESCRIPCION");
            this.Property(a => a.Nombre)
                .IsRequired()
                .HasColumnName("NOMBRE");
            this.Property(a => a.Nomenclatura)
                .HasColumnName("NOMENCLATURA");
            this.Property(a => a.ObjetoPadreId)
                .HasColumnName("ID_OBJETO_PADRE")
                .IsRequired();
            this.Property(a => a.PlanoId)
                .HasColumnName("ID_PLANO");
            this.Property(a => a.TipoDivisionId)
                .HasColumnName("ID_TIPO_DIVISION");
            this.Property(a => a.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");
            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");
            this.Property(a => a.UsuarioModificacionId)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");
            this.Property(a => a.FechaModificacion)
                .IsRequired()
                .IsConcurrencyToken()
                .HasColumnName("FECHA_MODIF");
            this.Property(a => a.UsuarioBajaId)
                .HasColumnName("ID_USU_BAJA");
            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

        }
    }
}
