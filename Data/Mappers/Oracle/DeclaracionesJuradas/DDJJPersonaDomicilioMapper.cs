using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class DDJJPersonaDomicilioMapper : EntityTypeConfiguration<DDJJPersonaDomicilio>
    {
        public DDJJPersonaDomicilioMapper()
        {
            this.ToTable("VAL_DDJJ_PERSONA_DOMICILIO");

            this.HasKey(a => a.IdPersonaDomicilio);

            this.Property(a => a.IdPersonaDomicilio)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DDJJ_PERSONA_DOMICILIO");

            this.Property(a => a.IdDominioTitular)
               .IsRequired()
               .HasColumnName("ID_DDJJ_DOMINIO_TITULAR");

            this.Property(a => a.IdDomicilio)
                .IsRequired()
                .HasColumnName("ID_DOMICILIO");

            this.Property(a => a.IdTipoDomicilio)
                .IsRequired()
                .HasColumnName("ID_TIPO_DOMICILIO");

            this.Property(a => a.IdUsuarioAlta)
               .IsRequired()
               .HasColumnName("ID_USU_ALTA");

            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.IdUsuarioModif)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            this.Property(a => a.FechaModif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.IdUsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            this.HasRequired(a => a.Domicilio)
              .WithMany()
              .HasForeignKey(a => a.IdDomicilio);

            this.HasRequired(a => a.DominioTitular)
              .WithMany(a => a.PersonaDomicilio)
              .HasForeignKey(a => a.IdDominioTitular);

            this.Ignore(a => a.Tipo);
            this.Ignore(a => a.Provincia);
            this.Ignore(a => a.Localidad);
            this.Ignore(a => a.Barrio);
            this.Ignore(a => a.Calle);
            this.Ignore(a => a.Altura);
            this.Ignore(a => a.Piso);
            this.Ignore(a => a.Departamento);
            this.Ignore(a => a.CodigoPostal);
            this.Ignore(a => a.Municipio);
            this.Ignore(a => a.Pais);
            this.Ignore(a => a.IdCalle);
        }
    }
}
