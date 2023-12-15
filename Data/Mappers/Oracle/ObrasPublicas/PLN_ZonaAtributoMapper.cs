using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Data.Mappers.Oracle.ObrasPublicas
{
    public class PLN_ZonaAtributoMapper : EntityTypeConfiguration<PLN_ZonaAtributo>
    {
        public PLN_ZonaAtributoMapper()
        {
            this.ToTable("PLN_ZONA_ATRIBUTO");

            this.Property(a => a.Id_Zona_Atributo)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_ZONA_ATRIBUTO");
            this.Property(a => a.Id_Atributo_Zona)
                .IsRequired()
                .HasColumnName("ID_ATRIBUTO_ZONA");
            this.Property(a => a.Valor)
                .HasMaxLength(50)
                .IsRequired()
                .HasColumnName("VALOR");
            this.Property(a => a.FeatId_Objeto)
                .IsRequired()
                .HasColumnName("FEATID_OBJETO");
            this.Property(a => a.Id_Usu_Alta)
               .IsRequired()
               .HasColumnName("ID_USU_ALTA");
            this.Property(a => a.Fecha_Alta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");
            this.Property(a => a.Id_Usu_Modif)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");
            this.Property(a => a.Fecha_Modif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");
            this.Property(a => a.Id_Usu_Baja)
                .HasColumnName("ID_USU_BAJA");
            this.Property(a => a.Fecha_Baja)
                .HasColumnName("FECHA_BAJA");
            this.Property(a => a.U_Medida)
                .HasMaxLength(10)
                .HasColumnName("U_MEDIDA");
            this.Property(a => a.Observaciones)
                .HasColumnName("OBSERVACIONES")
                .HasMaxLength(100)
                .IsOptional();

            this.HasKey(a => a.Id_Zona_Atributo);

            this.HasRequired(p => p.Atributo)
                .WithMany()
                .HasForeignKey(p => p.Id_Atributo_Zona);

            this.HasRequired(p => p.ObjetoAdministrativo)
                .WithMany()
                .HasForeignKey(p => p.FeatId_Objeto);
        }
    }
}
