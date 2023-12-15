using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class OCObjetoMapper : EntityTypeConfiguration<OCObjeto>
    {
        public OCObjetoMapper()
        {
            this.ToTable("OC_OBJETO");

            // Primary Key
            this.HasKey(a => a.FeatId);

            this.Property(a => a.FeatId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("FEATID");
            
            this.Property(a => a.ClassId)
                .HasColumnName("CLASSID");
            this.Property(a => a.Codigo)
                .HasColumnName("CODIGO");
            this.Property(a => a.Descripcion)
                .HasColumnName("DESCRIPCION");
            this.Property(a => a.Atributos)
                .HasColumnName("ATRIBUTOS");
            this.Property(a => a.Nombre)
                .HasColumnName("NOMBRE");                 
            this.Property(a => a.RevisionNumber)
                .HasColumnName("REVISIONNUMBER");
            this.Property(a => a.GeomTxt)
                .HasColumnName("GEOM_TXT");
            this.Property(a => a.IdSubtipoObjeto)
                .HasColumnName("ID_SUBTIPO_OBJETO")
                .IsRequired();
            this.Property(a => a.IdUsuarioAlta)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");
            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");
            this.Property(a => a.IdUsuarioModif)
                .HasColumnName("ID_USU_MODIF");
            this.Property(a => a.FechaModif)
                .HasColumnName("FECHA_MODIF");
            this.Property(a => a.IdUsuarioBaja)
                .HasColumnName("ID_USU_BAJA");
            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");
            
            this.Ignore(a => a.Geometry);
                 

        }
    }
}
