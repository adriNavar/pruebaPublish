using GeoSit.Data.BusinessEntities.Certificados;
using GeoSit.Data.BusinessEntities.LogRPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.Mappers.Oracle
{
    public class RPILogConsultasMapper : EntityTypeConfiguration<RPILogConsultas>
    {
        public RPILogConsultasMapper()
        {
            this.ToTable("RPI_LOG_CONSULTAS").HasKey(a => a.LogId);

            this.Property(a => a.LogId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_LOG");

            this.Property(a => a.UsuarioId)
                .IsRequired()
                .HasColumnName("ID_USUARIO");

            this.Property(a => a.Fecha)
                .IsRequired()
                .HasColumnName("FECHA");

            this.Property(a => a.TipoOperacionId)
                .IsRequired()
                .HasColumnName("ID_TIPO_OPERACION");

            this.Property(a => a.Valor)
                .IsRequired()
                .HasColumnName("VALOR");

            this.Property(a => a.CodigoRespuesta)
                .IsRequired()
                .HasColumnName("COD_RESP");


            //Relaciones
            this.HasRequired(p => p.TipoOperaciones)
                .WithMany()
                .HasForeignKey(p => p.TipoOperacionId);



        }
    }
}
