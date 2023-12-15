using GeoSit.Data.BusinessEntities.Inmuebles;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ParcelaGraficaMapper : EntityTypeConfiguration<ParcelaGrafica>
    {
        public ParcelaGraficaMapper()
        {
            this.ToTable("INM_PARCELA_GRAFICA")
                .HasKey(x => x.FeatID);

            this.Property(p => p.FeatID)
                .HasColumnName("FEATID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .IsRequired();

            this.Property(p => p.ParcelaID)
                .HasColumnName("ID_PARCELA")
                .IsOptional();

            this.Property(p => p.FechaAlta)
                .HasColumnName("FECHA_ALTA")
                .IsOptional();

            this.Property(p => p.IdOrigen)
                .HasColumnName("ID_ORIGEN")
                .IsOptional();

            this.Property(p => p.FechaBaja)
                .HasColumnName("FECHA_BAJA")
                .IsOptional();

            this.Property(p => p.FechaModificacion)
                .HasColumnName("FECHA_MODIF")
                .IsOptional();

            this.Property(p => p.UsuarioAltaID)
                .HasColumnName("ID_USU_ALTA")
                .IsOptional();

            this.Property(p => p.UsuarioBajaID)
                .HasColumnName("ID_USU_BAJA")
                .IsOptional();

            this.Property(p => p.UsuarioModificacionID)
                .HasColumnName("ID_USU_MODIF")
                .IsOptional();

            //this.Property(p => p.ClassID)
            //    .HasColumnName("CLASSID")
            //    .IsOptional();

            //this.Property(p => p.RevisionNumber)
            //    .HasColumnName("REVISIONNUMBER")
            //    .IsOptional();

            this.Ignore(p => p.ClassID);
            this.Ignore(p => p.RevisionNumber);
        }
    }
}
