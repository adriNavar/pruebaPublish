using GeoSit.Data.BusinessEntities.ValidacionesDB;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.ValidacionesDB
{
    public class ValidacionMapper : EntityTypeConfiguration<Validacion>
    {
        public ValidacionMapper()
        {
            ToTable("VL_VALIDACION")
                .HasKey(vl => vl.IdValidacion);

            Property(a => a.IdValidacion)
                .IsRequired()
                .HasColumnName("ID_VALIDACION");

            Property(a => a.IdTipoValidacion)
                .IsRequired()
                .HasColumnName("ID_TIPO_VALIDACION");

            Property(a => a.IdTipoObjeto)
                .IsRequired()
                .HasColumnName("ID_TIPO_OBJETO");

            Property(a => a.Sentencia)
                .IsRequired()
                .HasColumnName("SENTENCIA");

            Property(a => a.Mensaje)
                .IsRequired()
                .HasColumnName("MENSAJE");

            Property(a => a.Activa)
                .IsRequired()
                .HasColumnName("ACTIVA");

            Property(a => a.UsuarioAlta)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(a => a.UsuarioModif)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");
            
            Property(a => a.FechaModif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            Property(a => a.UsuarioBaja)
                .HasColumnName("ID_USU_BAJA");
            
            Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");


            HasMany(p => p.Funciones)
                .WithRequired()
                .HasForeignKey(p => p.IdFuncion);
        }
    }
}