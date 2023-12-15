using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.Mappers.Oracle
{
    public class UnidadTributariaDomicilioInmuebleMapper : EntityTypeConfiguration<UnidadTributariaDomicilioInmueble>
    {
        public UnidadTributariaDomicilioInmuebleMapper()
        {
            ToTable("INM_UT_DOMICILIO");

            HasKey(utdi => new { utdi.DomicilioInmuebleId, utdi.UnidadTributariaId });

            Property(utdi => utdi.DomicilioInmuebleId)
                .IsRequired()
                .HasColumnName("ID_DOMICILIO");

            Property(utdi => utdi.UnidadTributariaId)
                .IsRequired()
                .HasColumnName("ID_UNIDAD_TRIBUTARIA");

            Property(utdi => utdi.TipoDomicilioId)
                .IsRequired()
                .HasColumnName("ID_TIPO_DOMICILIO");

            //Relaciones
            //HasRequired(utdi => utdi.DomicilioInmueble)
            //    .WithMany(di => di.UnidadTributariaDomicilio)
            //    .HasForeignKey(utdi => utdi.DomicilioInmuebleId);

            //HasRequired(utdi => utdi.UnidadTributaria)
            //    .WithMany(ut => ut.UnidadTributariaDomicilios)
            //    .HasForeignKey(utdi => utdi.UnidadTributariaId);
        }
    }
}
