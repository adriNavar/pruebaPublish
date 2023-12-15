using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasParticulares;

namespace GeoSit.Data.Mappers.Oracle
{
    public class LiquidacionMapper : EntityTypeConfiguration<Liquidacion>
    {
        public LiquidacionMapper()
        {
            ToTable("OP_LIQUIDACIONES");

            HasKey(l => l.LiquidacionId);

            Property(l => l.LiquidacionId)                
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_LIQUIDACION");

            Property(l => l.Numero)
                .IsRequired()
                .HasColumnName("NUMERO");

            Property(l => l.Importe)
                .IsRequired()
                .HasColumnName("IMPORTE");

            Property(l => l.Fecha)
                .IsRequired()
                .HasColumnName("FECHA");

            Property(l => l.Observaciones)
                .HasMaxLength(4000)
                .IsUnicode(false)
                .IsOptional()
                .HasColumnName("OBSERVACIONES");

            Property(l => l.ExpedienteObraId)
                .IsOptional()
                .HasColumnName("ID_EXPEDIENTE");

            //Altas y bajas
            Property(l => l.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            Property(l => l.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(l => l.UsuarioModificacionId)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            Property(l => l.FechaModificacion)                
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            Property(l => l.UsuarioBajaId)
                .IsOptional()
                .HasColumnName("ID_USU_BAJA");

            Property(l => l.FechaBaja)
                .IsOptional()
                .HasColumnName("FECHA_BAJA");
            
            //Relaciones
            HasOptional(l => l.ExpedienteObra)
                .WithMany(eo => eo.Liquidaciones)
                .HasForeignKey(l => l.ExpedienteObraId);
        }
    }
}
