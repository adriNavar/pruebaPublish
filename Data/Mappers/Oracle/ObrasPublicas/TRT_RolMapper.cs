using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Data.Mappers.Oracle.ObrasPublicas
{
    public class TRT_RolMapper : EntityTypeConfiguration<TramiteRol>
    {
        public TRT_RolMapper()
        {
            this.ToTable("TRT_ROL");

            this.Property(a => a.Id_Rol)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_ROL");
            this.Property(a => a.Descripcion)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("DESCRIPCION");
            this.Property(a => a.Id_Usu_Alta)
                .HasColumnName("ID_USU_ALTA");
            this.Property(a => a.Fecha_Alta)
                .HasColumnName("FECHA_ALTA");
            this.Property(a => a.Id_Usu_Modif)
                .HasColumnName("ID_USU_MODIF");
            this.Property(a => a.Fecha_Modif)
                .HasColumnName("FECHA_MODIF");
            this.Property(a => a.Id_Usu_Baja)
                .HasColumnName("ID_USU_BAJA");
            this.Property(a => a.Fecha_Baja)
                .HasColumnName("FECHA_BAJA");

            this.HasKey(a => a.Id_Rol);
        }
    }
}
