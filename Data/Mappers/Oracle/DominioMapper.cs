using System.ComponentModel.DataAnnotations.Schema;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.Mappers.Oracle
{
    public class DominioMapper : AuditoriaEntityTypeConfiguration<Dominio>
    {
        public DominioMapper() : base()
        {
            ToTable("INM_DOMINIO");

            HasKey(d => d.DominioID);

            Property(d => d.DominioID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DOMINIO")
                .IsRequired();

            Property(d => d.UnidadTributariaID)
                .HasColumnName("ID_UNIDAD_TRIBUTARIA")
                .IsRequired();

            Property(d => d.TipoInscripcionID)
                .HasColumnName("ID_TIPO_INSCRIPCION")
                .IsRequired();

            Property(d => d.Inscripcion)
                .HasColumnName("INSCRIPCION")
                .IsRequired();

            Property(d => d.Fecha)
                .HasColumnName("FECHA")
                .IsRequired();

            Ignore(d => d.TipoInscripcionDescripcion);

            //Relaciones

            HasRequired(d => d.UnidadTributaria)
                .WithMany()
                .HasForeignKey(d => d.UnidadTributariaID);

            HasRequired(d => d.TipoInscripcion)
                .WithMany()
                .HasForeignKey(d => d.TipoInscripcionID);
        }
    }
}