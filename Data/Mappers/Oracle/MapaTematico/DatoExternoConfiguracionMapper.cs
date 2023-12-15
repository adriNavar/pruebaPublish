using GeoSit.Data.BusinessEntities.MapasTematicos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.Mappers.Oracle
{
    public class DatoExternoConfiguracionMapper : EntityTypeConfiguration<DatoExternoConfiguracion>
    {
        public DatoExternoConfiguracionMapper()
        {
            this.ToTable("MT_DATO_EXTERNO_CONFIG");

            this.Property(a => a.DatoExternoConfiguracionId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DATO_EXTERNO_CONFIG");
            this.Property(a => a.Componente)
                .IsRequired()
                .HasColumnName("ID_COMPONENTE");
            this.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("NOMBRE_ATRIBUTO");
            this.Property(a => a.TipoDato)
                .IsRequired()
                .HasColumnName("ID_TIPO_DATO");
            this.Property(a => a.Usuario)
                .IsRequired()
                .HasColumnName("ID_USUARIO");
                        
            this.HasKey(a => a.DatoExternoConfiguracionId);
        }
    }
}
