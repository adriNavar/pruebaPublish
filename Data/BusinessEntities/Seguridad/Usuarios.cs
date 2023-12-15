using System;
using System.Linq;

namespace GeoSit.Data.BusinessEntities.Seguridad
{
    public class Usuarios : IEntity
    {
        public long Id_Usuario { get; set; }
        public string Login { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Mail { get; set; }
        public string Sector { get; set; }
        public int? IdSector { get; set; }
        public string Id_tipo_doc { get; set; }
        public string Nro_doc { get; set; }
        public string Domicilio { get; set; }
        public bool Habilitado { get; set; }
        public bool Cambio_pass { get; set; }
        public long Usuario_alta { get; set; }
        public DateTime Fecha_alta { get; set; }
        public long Usuario_modificacion { get; set; }
        public DateTime Fecha_modificacion { get; set; }
        public long? Usuario_baja { get; set; }
        public DateTime? Fecha_baja { get; set; }
        public int? CantidadIngresosFallidos { get; set; }
        public DateTime? Fecha_Operacion { get; set; }

        //public ICollection<UsuariosRegistro> UsuariosRegistro { get; set; }
        public Sector SectorUsuario { get; set; }

        public string NombreApellidoCompleto { get { return string.Join(", ", (new[] { (Apellido ?? "").Trim(), (Nombre ?? "").Trim() }).Where(x => !string.IsNullOrEmpty(x))); } }

        public long? IdISICAT { get; set; }
        public string LoginISICAT { get; set; }
        public string NombreApellidoISICAT { get; set; }
        public DateTime? VigenciaDesdeISICAT { get; set; }
        public DateTime? VigenciaHastaISICAT { get; set; }

    }



}


