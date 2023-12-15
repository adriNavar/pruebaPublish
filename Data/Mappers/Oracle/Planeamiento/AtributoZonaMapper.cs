using GeoSit.Data.BusinessEntities.Planeamiento;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.Mappers.Oracle
{
    public class AtributoZonaMapper : EntityTypeConfiguration<AtributoZona>
    {
        public AtributoZonaMapper()
        {

            this.ToTable("PLN_ATRIBUTO_ZONA");

            this.Property(a => a.Id_Atributo_Zona)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_ATRIBUTO_ZONA");
            this.Property(a => a.Descripcion)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("DESCRIPCION");
            

            this.HasKey(a => a.Id_Atributo_Zona);
        }
    }
}
