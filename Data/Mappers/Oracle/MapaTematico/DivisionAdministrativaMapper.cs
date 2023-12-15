using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.Mappers.Oracle
{
    public class DivisionAdministrativaMapper : EntityTypeConfiguration<Division>
    {
        public DivisionAdministrativaMapper()
        {
            this.ToTable("OA_DIVISION");

            this.Property(a => a.FeatId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("FEATID");
            this.Property(a => a.Alias)
                .HasColumnName("ALIAS");
            this.Property(a => a.ClassId)
                .IsRequired()
                .HasColumnName("CLASSID");
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
            this.Property(a => a.RevisionNumber)
                .IsRequired()
                .HasColumnName("REVISIONNUMBER");
            //this.Property(a => a.TipoObjetoId)
            //    .HasColumnName("ID_TIPO_OBJETO")
            //    .IsRequired();
            this.Property(a => a.TipoDivisionId)
                .HasColumnName("IID_TIPO_DIVISION");
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

            this.Ignore(a => a.Geometry);
            this.Ignore(a => a.TipoObjetoId);
            // Primary Key
            this.HasKey(a => a.FeatId);
        }
    }
}
