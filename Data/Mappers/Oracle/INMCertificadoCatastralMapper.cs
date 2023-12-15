using GeoSit.Data.BusinessEntities.Certificados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.Mappers.Oracle
{
    public class INMCertificadoCatastralMapper : EntityTypeConfiguration<INMCertificadoCatastral>
    {
        public INMCertificadoCatastralMapper()
        {
            this.ToTable("INM_CERT_CAT");

            this.Property(a => a.CertificadoCatastralId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_CERT_CAT");

            /*this.Property(a => a.Numero)
                .HasColumnName("NUMERO");*/

            this.Property(a => a.FechaEmision)
                .HasColumnName("FECHA_EMISION");

            this.Property(a => a.UnidadTributariaId)
                .IsRequired()
                .HasColumnName("ID_UNIDAD_TRIBUTARIA");

            this.Property(a => a.SolicitanteId)
                .IsRequired()
                .HasColumnName("ID_SOLICITANTE");

            this.Property(a => a.Observaciones)
                .HasColumnName("OBSERVACIONES");

            /*this.Property(a => a.Medidas)
                .HasColumnName("MEDIDAS");

            this.Property(a => a.Linderos)
                .HasColumnName("LINDEROS");*/

            this.Property(a => a.Descripcion)
                .HasColumnName("DESCRIPCION");

             this.Property(a => a.MensuraId)
                .IsRequired()
                .HasColumnName("ID_MENSURA");

            /*this.Property(a => a.MensuraVepId)
                .HasColumnName("ID_MENSURA_VEP");*/


            this.Property(a => a.UsuarioAltaId)
                .HasColumnName("ID_USU_ALTA");


            this.Property(a => a.FechaAlta)
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.UsuarioModifId)
                .HasColumnName("ID_USU_MODIF");

            this.Property(a => a.FechaModif)
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.UsuarioBajaId)
                .HasColumnName("ID_USU_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");


            this.Property(a => a.Motivo)
                .IsRequired()
                .HasColumnName("MOTIVO");

            this.Property(a => a.Vigencia)
                .HasColumnName("VIGENCIA");

            this.HasKey(a => a.CertificadoCatastralId);

            //Relaciones
            HasRequired(p => p.UnidadTributaria)
                .WithMany()
                .HasForeignKey(p => p.UnidadTributariaId);

            HasRequired(p => p.Solicitante)
                .WithMany()
                .HasForeignKey(d => d.SolicitanteId);

            this.Ignore(x => x.MensuraDesc);
            //this.Ignore(x => x.MensuraVepDesc);
            this.Ignore(x => x.Designacion);
            this.Ignore(x => x.FechaVigencia);
            this.Ignore(x => x.CodigoProvincial);
            this.Ignore(x => x.Numero);
        }
    }
}
