using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Data.Mappers.Oracle.ObrasPublicas
{
    public class ObjetoInfraestructuraMapper : EntityTypeConfiguration<ObjetoInfraestructura>
    {
        public ObjetoInfraestructuraMapper()
        {
            this.ToTable("INF_OBJETO");

            this.Property(a => a.FeatID)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("FEATID");
            this.Property(a => a.Nombre)
                .IsRequired()
                .HasColumnName("NOMBRE");
            this.Property(a => a.Descripcion)
                .IsRequired()
                .HasColumnName("DESCRIPCION");
            this.Property(a => a.ID_Objeto_Padre)
                .HasColumnName("ID_OBJETO_PADRE");
            this.Property(a => a.ID_Subtipo_Objeto)
                .HasColumnName("ID_SUBTIPO_OBJETO");
            this.Property(a => a.ID_Usu_Alta)
                .HasColumnName("ID_USU_ALTA");
            this.Property(a => a.Fecha_Alta)
                .HasColumnName("FECHA_ALTA");
            this.Property(a => a.ID_Usu_Modif)
                .HasColumnName("ID_USU_MODIF");
            this.Property(a => a.Fecha_Modif)
                .HasColumnName("FECHA_MODIF");
            this.Property(a => a.ID_Usu_Baja)
                .HasColumnName("ID_USU_BAJA");
            this.Property(a => a.Fecha_Baja)
                .HasColumnName("FECHA_BAJA");
            this.Property(a => a.Atributos)
                .HasColumnName("ATRIBUTOS");
            this.Property(a => a.ClassID)
                .HasColumnName("CLASSID");
            this.Property(a => a.RevisionNumber)
                .HasColumnName("REVISIONNUMBER");

            this.Ignore(a => a.WKT);
            this.HasKey(a => a.FeatID);
        }
    }
}
