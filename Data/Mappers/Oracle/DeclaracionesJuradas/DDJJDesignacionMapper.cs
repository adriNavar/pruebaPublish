using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class DDJJDesignacionMapper : EntityTypeConfiguration<DDJJDesignacion>
    {
        public DDJJDesignacionMapper()
        {
            this.ToTable("VAL_DDJJ_DESIGNACION");

            this.HasKey(a => a.IdDesignacion);

            this.Property(a => a.IdDesignacion)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DDJJ_DESIGNACION");

            this.Property(a => a.IdDeclaracionJurada)
               .IsRequired()
               .HasColumnName("ID_DDJJ");

            this.Property(a => a.IdTipoDesignador)
                .IsRequired()
                .HasColumnName("ID_TIPO_DESIGNADOR");

            this.Property(a => a.IdCalle)
            .HasColumnName("ID_CALLE");

            this.Property(a => a.Calle)
               .HasColumnName("CALLE");

            this.Property(a => a.Numero)
               .HasColumnName("NUMERO");

            this.Property(a => a.IdBarrio)
               .HasColumnName("ID_BARRIO");

            this.Property(a => a.Barrio)
               .HasColumnName("BARRIO");

            this.Property(a => a.IdLocalidad)
                .IsRequired()
               .HasColumnName("ID_LOCALIDAD");

            this.Property(a => a.Localidad)
               .HasColumnName("LOCALIDAD");

            this.Property(a => a.IdDepartamento)
                .IsRequired()
               .HasColumnName("ID_DEPARTAMENTO");

            this.Property(a => a.Departamento)
               .HasColumnName("DEPARTAMENTO");

            this.Property(a => a.IdParaje)
               .HasColumnName("ID_PARAJE");

            this.Property(a => a.Paraje)
               .HasColumnName("PARAJE");

            this.Property(a => a.IdSeccion)
               .HasColumnName("ID_SECCION");

            this.Property(a => a.Seccion)
               .HasColumnName("SECCION");

            this.Property(a => a.Chacra)
               .HasColumnName("CHACRA");

            this.Property(a => a.Quinta)
               .HasColumnName("QUINTA");

            this.Property(a => a.Fraccion)
               .HasColumnName("FRACCION");

            this.Property(a => a.IdManzana)
               .HasColumnName("ID_MANZANA");

            this.Property(a => a.Manzana)
               .HasColumnName("MANZANA");

            this.Property(a => a.Lote)
               .HasColumnName("LOTE");

            this.Property(a => a.CodigoPostal)
               .HasColumnName("CODIGO_POSTAL");   

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

            this.HasRequired(a => a.DeclaracionJurada)
               .WithMany(a => a.Designacion)
               .HasForeignKey(a => a.IdDeclaracionJurada);
        }
    }
}
