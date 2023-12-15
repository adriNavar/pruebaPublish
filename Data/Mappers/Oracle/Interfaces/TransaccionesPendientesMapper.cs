using GeoSit.Data.BusinessEntities.Interfaces;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TransaccionesPendientesMapper : EntityTypeConfiguration<TransaccionesPendientes>
    {
        public TransaccionesPendientesMapper()
        {

            this.ToTable("VR02201$TLW$GEOSYS");

            this.Property(a => a.Mi_Leyenda)
                //.HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasMaxLength(80)
                .HasColumnName("MI_IMP_LEYENDA");
            this.Property(a => a.Mi_Identificacion)
                .HasMaxLength(80)
                .HasColumnName("MI_IMP_IDENTIFICACION");
            this.Property(a => a.Mi_Definitivo)
                .HasColumnName("MI_IMP_DEFINITIVO");
            this.Property(a => a.Otro_Tipo)
                .HasColumnName("OTRO_IMP_TIPO");
            this.Property(a => a.Otro_Leyenda)
                .HasMaxLength(80)
                .HasColumnName("OTRO_IMP_LEYENDA");
            this.Property(a => a.Otro_Identificacion)
                .HasMaxLength(80)
                //.IsUnicode(false)
                .HasColumnName("OTRO_IMP_IDENTIFICACION");
            this.Property(a => a.Otro_Nombre)
                .HasMaxLength(80)
                //.IsUnicode(false)
                .HasColumnName("OTRO_IMP_NOMBRE");
            this.Property(a => a.Otro_Definitivo)
                .HasColumnName("OTRO_IMP_DEFINITIVO");
            this.Property(a => a.Es)
                .HasMaxLength(5)
                .HasColumnName("ES");
            this.Property(a => a.Relacion)
                .HasMaxLength(80)
                .HasColumnName("RELACION");
            this.Property(a => a.Alta_Fecha)
                .HasColumnName("ALTA_FECHA");
            this.Property(a => a.Fecha_Desde)
                .HasColumnName("FECHA_DESDE");
            this.Property(a => a.Fecha_Hasta)
                .HasColumnName("FECHA_HASTA");

            this.HasKey(a => a.Otro_Nombre);
            this.Ignore(a => a.IdTransaccion);
            this.Ignore(a => a.listaDestino);
            this.Ignore(a => a.listaOrigen);
            this.Ignore(a => a.ParcelaID);
        }
    }
}
