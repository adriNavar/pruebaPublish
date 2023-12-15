using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class VALPuntajesLocalidadesMapper : EntityTypeConfiguration<VALPuntajesLocalidades>
    {
        public VALPuntajesLocalidadesMapper()
        {
            this.ToTable("VAL_PUNTAJES_LOCALIDADES");

            this.HasKey(a => a.IdPuntajeLocalidad);

            this.Property(a => a.IdPuntajeLocalidad)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_PUNTAJE_LOCALIDAD");

            this.Property(a => a.IdLocalidad)
               .IsRequired()
               .HasColumnName("ID_LOCALIDAD");

            this.Property(a => a.DistanciaMinima)
            .HasColumnName("DISTANCIA_MINIMA");

            this.Property(a => a.DistanciaMaxima)
                .HasColumnName("DISTANCIA_MAXIMA");

            this.Property(a => a.Puntaje)                
                .HasColumnName("PUNTAJE");

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
        }
    }
}
