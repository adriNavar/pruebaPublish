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
    public class ObjetoResultadoMapper : EntityTypeConfiguration<ObjetoResultado>
    {
        public ObjetoResultadoMapper()
        {
            this.ToTable("MT_OBJETO_RESULTADO");

            this.Property(a => a.FeatId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("FEATID");
            this.Property(a => a.ClassId)
                .IsRequired()
                .HasColumnName("CLASSID");
            this.Property(a => a.RevisionNumber)
                .IsRequired()
                .HasColumnName("REVISIONNUMBER");
            this.Property(a => a.GeometryType)
                .IsOptional()
                .HasColumnName("GEOMETRY_TYPE");
            this.Property(a => a.GUID)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("GUID");
            this.Property(a => a.IdOrigin)
                .IsRequired()
                .HasColumnName("ID_ORIGIN");
            this.Property(a => a.Descripcion)
                .IsOptional()
                .HasColumnName("DESCRIPCION");
            this.Property(a => a.Valor)
                .IsOptional()
                .IsUnicode(false)
                .HasColumnName("VALOR");
            this.Property(a => a.Rango)
                .IsOptional()
                .HasColumnName("RANGO");
            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.Ignore(a => a.Geom);
            this.Ignore(a => a.WKT);

            this.HasKey(a => a.FeatId);


        }
    }
}
