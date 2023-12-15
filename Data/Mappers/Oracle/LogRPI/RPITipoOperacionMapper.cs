using GeoSit.Data.BusinessEntities.Certificados;
using GeoSit.Data.BusinessEntities.LogRPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.Mappers.Oracle
{
    public class RPITipoOperacionMapper : EntityTypeConfiguration<RPITipoOperacion>
    {
        public RPITipoOperacionMapper()
        {
            this.ToTable("RPI_TIPO_OPERACION").HasKey(a => a.TipoOperacionId);

            this.Property(a => a.TipoOperacionId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_TIPO_OPERACION");

            this.Property(a => a.Descripcion)
                .IsRequired()
                .HasColumnName("DESCRIPCION");

            this.Property(a => a.Sentido)
                .IsRequired()
                .HasColumnName("SENTIDO");

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


        }
    }
}
