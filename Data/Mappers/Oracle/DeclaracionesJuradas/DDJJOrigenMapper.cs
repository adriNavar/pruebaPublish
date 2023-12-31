﻿using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class DDJJOrigenMapper : EntityTypeConfiguration<DDJJOrigen>
    {
        public DDJJOrigenMapper()
        {
            this.ToTable("VAL_DDJJ_ORIGEN");

            this.HasKey(a => a.IdOrigen);

            this.Property(a => a.IdOrigen)
                .IsRequired()
                .HasColumnName("ID_DDJJ_ORIGEN");

            this.Property(a => a.Descripcion)
               .IsRequired()
               .HasColumnName("DESCRIPCION");

            this.Property(a => a.IdUsuarioAlta)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.IdUsuarioModif)
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
