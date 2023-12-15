using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;

namespace GeoSit.Data.Mappers.Oracle.Temporal.DDJJ
{
    public class INMMejoraTemporalMapper : TablaTemporalMapper<INMMejoraTemporal>
    {
        public INMMejoraTemporalMapper()
            : base("INM_MEJORAS")
        {
            HasKey(a => a.IdMejora);

            Property(a => a.IdMejora)
                .IsRequired()
                .HasColumnName("ID_MEJORA");

            Property(a => a.IdEstadoConservacion)
                .HasColumnName("ID_ESTADO_CONSERVACION");

            Property(a => a.IdDestinoMejora)
                .HasColumnName("ID_DESTINO_MEJORA");

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

            HasMany(a => a.OtrasCar)
                .WithRequired()
                .HasForeignKey(a => a.IdMejora)
                .WillCascadeOnDelete();

            HasMany(a => a.MejorasCar)
                .WithRequired()
                .HasForeignKey(a => a.IdMejora)
                .WillCascadeOnDelete();
        }
    }
}
