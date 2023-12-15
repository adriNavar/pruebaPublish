using GeoSit.Data.BusinessEntities.Seguridad;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class FuncionesMapper : EntityTypeConfiguration<Funciones>
    {
        public FuncionesMapper()
        {
            this.ToTable("SE_FUNCION");

            this.Property(a => a.Id_Funcion)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_FUNCION");
            this.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
            this.Property(a => a.Id_Aplicacion)
                .IsRequired()
                .HasColumnName("ID_APLICACION");            
            this.Property(a => a.Id_Funcion_Padre)
                .IsRequired()
                .HasColumnName("ID_FUNCION_PADRE");

            this.HasKey(a => a.Id_Funcion);

        }
    }
}
