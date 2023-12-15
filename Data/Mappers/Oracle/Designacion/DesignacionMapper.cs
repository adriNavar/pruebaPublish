using GeoSit.Data.BusinessEntities.Designaciones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class DesignacionMapper : EntityTypeConfiguration<Designacion>
    {
        public DesignacionMapper()
        {
            this.ToTable("INM_DESIGNACION");

            this.HasKey(a => a.IdDesignacion);

            this.Property(a => a.IdDesignacion)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DESIGNACION");

            this.Property(a => a.IdTipoDesignador)
                .IsRequired()
                .HasColumnName("ID_TIPO_DESIGNADOR");

            this.Property(a => a.IdParcela)
             .IsRequired()
             .HasColumnName("ID_PARCELA");

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
               .HasColumnName("ID_LOCALIDAD");

            this.Property(a => a.Localidad)
               .HasColumnName("LOCALIDAD");

            this.Property(a => a.IdDepartamento)
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

            this.HasRequired(x => x.TipoDesignador)
                .WithMany()
                .HasForeignKey(x => x.IdTipoDesignador);
        }
    }
}
