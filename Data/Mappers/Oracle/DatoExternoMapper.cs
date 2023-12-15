﻿using GeoSit.Data.BusinessEntities.MapasTematicos;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class DatoExternoMapper : EntityTypeConfiguration<DatoExterno>
    {
        public DatoExternoMapper()
        {
            this.ToTable("MT_DATO_EXTERNO");

            this.Property(a => a.DatoExternoId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DATO_EXTERNO");
            this.Property(a => a.DatoExternoConfiguracionId)
                .IsRequired()
                .HasColumnName("ID_DATO_EXTERNO_CONFIG");
            this.Property(a => a.idComponente)
                .IsRequired()
                .HasColumnName("COMPONENTE_ID");
            this.Property(a => a.Valor)
                .HasMaxLength(200)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("VALOR");

            //this.HasKey(a => new { a.DatoExternoConfiguracionId, a.idComponente });
            this.HasKey(a => a.DatoExternoId);
        }
    }
}
