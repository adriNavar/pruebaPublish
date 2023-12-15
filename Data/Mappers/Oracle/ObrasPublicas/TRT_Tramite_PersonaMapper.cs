using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Data.Mappers.Oracle.ObrasPublicas
{
    public class TRT_Tramite_PersonaMapper : EntityTypeConfiguration<TramitePersona>
    {
        public TRT_Tramite_PersonaMapper()
        {
            this.ToTable("TRT_TRAMITE_PERSONA");

            this.Property(a => a.Id_Tramite_Persona)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_TRAMITE_PERSONA");
            this.Property(a => a.Id_Tramite)
                .IsRequired()
                .HasColumnName("ID_TRAMITE");
            this.Property(a => a.Id_Persona)
                .IsRequired()
                .HasColumnName("ID_PERSONA");
            this.Property(a => a.Id_Rol)
                .HasColumnName("ID_ROL");
            this.Property(a => a.Id_Usu_Alta)
                .HasColumnName("ID_USU_ALTA");
            this.Property(a => a.Fecha_Alta)
                .HasColumnName("FECHA_ALTA");
            this.Property(a => a.Id_Usu_Modif)
                .HasColumnName("ID_USU_MODIF");
            this.Property(a => a.Fecha_Modif)
                .HasColumnName("FECHA_MODIF");
            this.Property(a => a.Id_Usu_Baja)
                .HasColumnName("ID_USU_BAJA");
            this.Property(a => a.Fecha_Baja)
                .HasColumnName("FECHA_BAJA");

            this.HasKey(a => a.Id_Tramite_Persona);

            HasRequired(a => a.Tramite)
                .WithMany(p => p.Personas)
                .HasForeignKey(a => a.Id_Tramite);

            HasRequired(a => a.Persona)
                .WithMany()
                .HasForeignKey(a => a.Id_Persona);

            HasRequired(a => a.Rol)
                .WithMany()
                .HasForeignKey(a => a.Id_Rol);
        }

    }
}
