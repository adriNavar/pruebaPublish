using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel;

namespace GeoSit.Client.Web.Models
{

    public class SeguridadModel
    {
        public SeguridadModel()
        {
            ParametrosGenerales = new ParametrosGeneralesModel();
            Mensaje = "";
            Usuarios = new UsuariosModel();
            TipoDoc = new TipoDocModel();
            UsuariosRegistro = new UsuariosRegistroModel();
            Perfiles = new PerfilesModel();
            Horarios = new HorariosModel();
            Funciones = new FuncionesModel();
            Entornos = new EntornosModel();
            PerfilesFunciones = new PerfilesFuncionesModel();
            PerfilesComponentes = new PerfilesComponentesModel();
            Consultas = new ReportesModel();
        }
        public ParametrosGeneralesModel ParametrosGenerales { get; set; }
        public string Mensaje { get; set; }
        public UsuariosModel Usuarios { get; set; }
        public UsuariosHistModel UsuariosHist { get; set; }
        public UsuariosPerfilesModel UsuariosPerfiles { get; set; }
        public TipoDocModel TipoDoc { get; set; }
        public UsuariosRegistroModel UsuariosRegistro { get; set; }
        public PerfilesModel Perfiles { get; set; }
        public DistritosModel Distritos { get; set; }
        public HorariosModel Horarios { get; set; }
        public FuncionesModel Funciones { get; set; }
        public EntornosModel Entornos { get; set; }
        public PerfilesFuncionesModel PerfilesFunciones { get; set; }
        public PerfilesComponentesModel PerfilesComponentes { get; set; }
        public ReportesModel Consultas { get; set; }

    }
    public class ParametrosGeneralesModel
    {
        public long Id_Parametro { get; set; }
        public string Clave { get; set; }
        public string Valor { get; set; }
        public string Descripcion { get; set; }
        public string Agrupador { get; set; }

        /* parametros Carrier */
        public string LimitesAccesos { get; set; }
        public string LimitesClaves { get; set; }
        public string ConexionesDesde { get; set; }
        public string IntentosDesde { get; set; }
        public string InactividadDesde { get; set; }
        public string HabilitaMail { get; set; }
        public string Email { get; set; }
        public string VigenciaDesde { get; set; }
        public string CantidadDiasPassDesde { get; set; }
        public string CantidadAlmacenadaDesde { get; set; }
        public string LongitudDesde { get; set; }
        public string NivelLetras { get; set; }
        public string NivelNumeros { get; set; }
        public string NivelEspeciales { get; set; }
        public string NivelMayusculas { get; set; }
        public string NivelMinusculas { get; set; }
        public string Active_Directory { get; set; }

    }

    public class SectorModel
    {
        public int IdSector { get; set; }
        public string Nombre { get; set; }

        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
    public class UsuariosModel
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
        public string Fecha_alta_String { get; set; }
        public long Usuario_modificacion { get; set; }
        public DateTime Fecha_modificacion { get; set; }
        public string Fecha_modificacion_String { get; set; }
        public long? Usuario_baja { get; set; }
        public DateTime? Fecha_baja { get; set; }
        public string Fecha_baja_string { get; set; }
        public string Token { get; set; }
        public DateTime? Fecha_Operacion { get; set; }
        public int? CantidadIngresosFallidos { get; set; }
        public string Ip { get; set; }
        public string Machine_Name { get; set; }

        public SectorModel SectorUsuario { get; set; }
    }

    public class UsuariosHistModel
    {
        public long Id_Usuario_Hist { get; set; }
        public long Id_Usuario { get; set; }
        public string Login { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Mail { get; set; }
        public string Sector { get; set; }
        public string Id_tipo_doc { get; set; }
        public string Nro_doc { get; set; }
        public string Domicilio { get; set; }
        public long? Habilitado { get; set; }
        public long? Cambio_pass { get; set; }
        public long Usuario_Operacion { get; set; }
        public DateTime Fecha_Operacion { get; set; }

    }
    public class TipoDocModel
    {

        public long Id_Tipo_Doc_Ident { get; set; }
        public string Descripcion { get; set; }

    }
    public class UsuariosRegistroModel
    {
        public long Id_Usuario_Registro { get; set; }
        public long Id_Usuario { get; set; }
        public string Registro { get; set; }
        public long Usuario_Operacion { get; set; }
        public DateTime Fecha_Operacion { get; set; }
    }
    public class UsuariosPerfilesModel
    {
        public long Id_Usuario_Perfil { get; set; }
        public long Id_Usuario { get; set; }
        public long Id_Perfil { get; set; }
        public long Id_Horario { get; set; }
        public long Usuario_Alta { get; set; }
        public DateTime Fecha_Alta { get; set; }
        public long? Usuario_Baja { get; set; }
        public DateTime? Fecha_Baja { get; set; }
        public HorariosModel Horarios { get; set; }

    }
    public class PerfilesModel
    {
        public long Id_Perfil { get; set; }
        public string Nombre { get; set; }
        public long Id_Horario { get; set; }
        public string HorarioDescripcion { get; set; }
        public long Usuario_Alta { get; set; }
        public DateTime Fecha_Alta { get; set; }
        public long Usuario_Modificacion { get; set; }
        public DateTime Fecha_Modificacion { get; set; }
        public long? Usuario_Baja { get; set; }
        public DateTime? Fecha_Baja { get; set; }
        public HorariosModel Horario { get; set; }

    }

    public class UsuariosDistritosModel
    {
        public long Id_Usuario_Distrito { get; set; }
        public long Id_Usuario { get; set; }
        public string Id_Distrito { get; set; }
        public DateTime? Fecha_Baja { get; set; }
        public long? Usuario_Baja { get; set; }
        public DateTime? Fecha_Alta { get; set; }
        public long? Usuario_Alta { get; set; }

    }

    public class FeriadosModel
    {
        public long Id_Feriado { get; set; }
        [DisplayName("Dia")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        [DisplayName("Descripción")]
        public String Descripcion { get; set; }
        public long Usuario_Alta { get; set; }
        public DateTime Fecha_Alta { get; set; }
        public long? Usuario_Baja { get; set; }
        public DateTime? Fecha_Baja { get; set; }

    }

    //NUEVO DISTRITOS MODEL!
    public class DistritosModel
    {
        public string Id_Distrito { get; set; }
        public string Geometry { get; set; }
        public string Nombre { get; set; }
        public string Abrev { get; set; }
        public long Id_Region { get; set; }
        public long? Id_Provincia { get; set; }
        public long? Prestacion { get; set; }
        public long? Apic_Gid { get; set; }
        public long? Apic_Id { get; set; }
        public RegionesModel Region { get; set; }

    }
    //NUEVO REGIONES MODEL!
    public class RegionesModel
    {
        public long Id_Region { get; set; }
        public string Nombre { get; set; }
        public string Geometry { get; set; }
        public long Id_Concesion { get; set; }
        public long? Apic_Gid { get; set; }
        public string Apic_Id { get; set; }
        public ICollection<DistritosModel> Distritos { get; set; }

    }

    public class HorariosModel
    {
        public HorariosModel()
        {
            HorariosDetalle = new List<HorariosDetalleModel>();
        }
        public long Id_Horario { get; set; }
        [DisplayName("Nombre")]
        public string Descripcion { get; set; }
        public long Usuario_Alta { get; set; }
        public DateTime Fecha_Alta { get; set; }
        public long Usuario_Modificacion { get; set; }
        public DateTime Fecha_Modificacion { get; set; }
        public long? Usuario_Baja { get; set; }
        public DateTime? Fecha_Baja { get; set; }
        public ICollection<PerfilesModel> Perfiles { get; set; }
        public List<HorariosDetalleModel> HorariosDetalle { get; set; }

        public string Mensaje { get; set; }
    }

    public class HorariosDetalleModel
    {
        public long Id_Horario_Detalle { get; set; }
        public long Id_Horario { get; set; }
        public string Dia { get; set; }
        [DisplayName("Horario Desde")]
        [DisplayFormat(DataFormatString = "{0:HH:mm}")]
        public DateTime Hora_Inicio { get; set; }
        [DisplayName("Horario Hasta")]
        [DisplayFormat(DataFormatString = "{0:HH:mm}")]
        public DateTime Hora_Fin { get; set; }
        public HorariosModel Horario { get; set; }
    }

    public class ValidarUsuarioModel
    {
        [Required]
        public string Nro_doc { get; set; }
        public string Mensaje { get; set; }
    }
    public class FuncionesModel
    {
        public long Id_Funcion { get; set; }
        public string Nombre { get; set; }
        public long Id_Aplicacion { get; set; }
        public long Id_Funcion_Padre { get; set; }
    }

    public class EntornosModel
    {
        public long Id_Entorno { get; set; }
        public string Nombre { get; set; }
    }
    public class PerfilesFuncionesModel
    {
        public long Id_Perfil_Funcion { get; set; }
        public long Id_Perfil { get; set; }
        public long Id_Funcion { get; set; }
        public long Usuario_Alta { get; set; }
        public DateTime Fecha_Alta { get; set; }
        public long? Usuario_Baja { get; set; }
        public DateTime? Fecha_Baja { get; set; }

    }
    public class PerfilesComponentesModel
    {
        public long Id_Perfil_Comp { get; set; }
        public long Id_Perfil { get; set; }
        public long Id_Componente { get; set; }
        public long Usuario_Alta { get; set; }
        public DateTime Fecha_Alta { get; set; }
        public long? Usuario_Baja { get; set; }
        public DateTime? Fecha_Baja { get; set; }

    }

    public class PerfilesUsuariosModel
    {
        public long Id_Perfil { get; set; }
        public long Id_Usuario { get; set; }
        public string Login { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public long Cantidad_Perfiles { get; set; }

    }

    public class ReportesModel
    {
        public long idUsuarios { get; set; }
        public long FuncionAsociada { get; set; }
        public DateTime fechaDesde { get; set; }
        public DateTime fechaHasta { get; set; }

    }

    public class Nodo
    {
        public bool esRoot { get; set; }
        public long id { get; set; }
        public long padre { get; set; }
        public string nombre { get; set; }
        public List<Nodo> hijos { get; set; }

        public void PrintPretty(string indent, bool last)
        {

            System.Diagnostics.Debug.Write(indent);
            if (last)
            {
                System.Diagnostics.Debug.Write("\\-");
                indent += "  ";
            }
            else
            {
                System.Diagnostics.Debug.Write("|-");
                indent += "| ";
            }
            System.Diagnostics.Debug.Write(nombre + Environment.NewLine);

            if (hijos != null)
            {
                for (int i = 0; i < hijos.Count; i++)
                {
                    hijos[i].PrintPretty(indent, i == hijos.Count - 1);
                }
            }
        }

    }

}
