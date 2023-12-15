using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoPlanoMapper : EntityTypeConfiguration<TipoPlano>
    {
        public TipoPlanoMapper()
        {
            //Table mapping
            ToTable("GR_TIPO_PLANO");

            //Primary key
            HasKey(p => p.IdTipoPlano);

            //Properties
            Property(p => p.IdTipoPlano)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_TIPO_PLANO");

            Property(p => p.Nombre)
                .IsRequired()
                .HasColumnName("NOMBRE");

            Property(p => p.IdPlantilla)
                .IsRequired()
                .HasColumnName("ID_PLANTILLA");

            Property(p => p.Activo)
                .IsRequired()
                .HasColumnName("ACTIVO");

            Property(p => p.Tema)
                .HasColumnName("TEMA");

            Property(p => p.Servicio)
                .HasColumnName("SERVICIO");

            Property(p => p.CodigoPlano)
                .HasColumnName("CODIGO_PLANO");

            Property(p => p.IdUsuarioAlta)
                .IsRequired()
                .HasColumnName("USUARIO_ALTA");

            Property(p => p.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(p => p.IdUsuarioModificacion)
                .IsRequired()
                .HasColumnName("USUARIO_MODIFICACION");

            Property(p => p.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIFICACION");

            Property(p => p.IdUsuarioBaja)
                .IsOptional()
                .HasColumnName("USUARIO_BAJA");

            Property(p => p.FechaBaja)
                .IsOptional()
                .HasColumnName("FECHA_BAJA");

            ////Non persistent fields
            //Ignore(p => p.Pen);
            //Ignore(p => p.Brush);
            //Ignore(p => p.FillBrush);
            //Ignore(p => p.PuntoImagen);
            //Ignore(p => p.PuntoImagenFormat);

            ////Relationship with Plantilla
            //HasRequired(p => p.Plantilla)
            //    .WithMany(p => p.Layers)
            //    .HasForeignKey(p => p.IdPlantilla);

            //Relationship with Componente
            //HasRequired(p => p.Componente)
            //    .WithMany(c => c.Layers)
            //    .HasForeignKey(p => p.ComponenteId);
        }
    }
}
