﻿using GeoSit.Data.BusinessEntities.MesaEntradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class MEObjetoTramiteMapper : EntityTypeConfiguration<MEObjetoTramite>
    {
        public MEObjetoTramiteMapper()
        {
            this.ToTable("ME_OBJETO_TRAMITE");

            this.HasKey(a => a.IdObjetoTramite);

            this.Property(a => a.IdObjetoTramite)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_OBJETO_TRAMITE");

            this.Property(a => a.Descripcion)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DESCRIPCION");

            this.Property(a => a.IdTipoTramite)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_TIPO_TRAMITE");

            this.Property(a => a.Plantilla)
                .IsRequired()
                .HasColumnName("PLANTILLA");

            this.Property(a => a.UsuarioAlta)
                .IsRequired()
                .HasColumnName("USUARIO_ALTA");

            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.UsuarioModif)
                .IsRequired()
                .HasColumnName("USUARIO_MODIF");

            this.Property(a => a.FechaModif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.UsuarioBaja)
                .HasColumnName("USUARIO_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");
        }
    }
}
