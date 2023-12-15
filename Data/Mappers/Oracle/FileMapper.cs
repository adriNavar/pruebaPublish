using GeoSit.Data.BusinessEntities.MapasTematicos;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class FileDataMapper : EntityTypeConfiguration<FileData>
    {
        public FileDataMapper()
        {

            this.ToTable("MT_FILE_DATA");

            this.Property(a => a.FileDataId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_FILE_DATA");
            this.Property(a => a.Valor)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("VALOR");
            this.Property(a => a.FileColumnId)
                .IsRequired()
                .HasColumnName("ID_FILE_COLUMN");
            this.Property(a => a.IndiceFila)
                .IsRequired()
                .HasColumnName("INDICE_FILA");

            this.HasKey(a => a.FileDataId);
        }
    }
    public class FileColumnMapper : EntityTypeConfiguration<FileColumn>
    {
        public FileColumnMapper()
        {

            this.ToTable("MT_FILE_COLUMN");

            this.Property(a => a.FileColumnId)
                 .IsRequired()
                 .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                 .HasColumnName("ID_FILE_COLUMN");
            this.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
            this.Property(a => a.FileDescriptorId)
                .IsRequired()
                .HasColumnName("ID_FILE_DESCRIPTOR");
            this.Property(a => a.IndiceColumna)
                .IsRequired()
                .HasColumnName("INDICE_COLUMNA");
            this.Property(a => a.TipoDato)
                .IsRequired()
                .HasColumnName("ID_TIPO_DATO");

            this.HasKey(a => a.FileColumnId);
        }
    }
    public class FileDescriptorMapper : EntityTypeConfiguration<FileDescriptor>
    {
        public FileDescriptorMapper()
        {

            this.ToTable("MT_FILE_DESCRIPTOR");

            this.Property(a => a.FileDescriptorId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_FILE_DESCRIPTOR");
            this.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
            this.Property(a => a.Path)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("PATH");

            this.HasKey(a => a.FileDescriptorId);
        }
    }
}
