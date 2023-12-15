using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Interfaces
{
    public class DatosGenerales
    {
        public String Catastro_Id{ get; set; }
        public String Nomenclatura_Cat { get; set; }
        public long Nom_Nueva_Circuns { get; set; }
        public long Nom_Nueva_Sector { get; set; }
        public long Nom_Nueva_Chacra{ get; set; }
        public String Nom_Nueva_Codqmf { get; set; }
        public long Nom_Nueva_Qmf { get; set; }
        public long Nom_Nueva_Parcela { get; set; }
        public String Nom_Nueva_Macizo { get; set; }
        public String Estado { get; set; }
        public String Tit_Identificacion { get; set; }
        public String Tit_Nombre { get; set; }
        public String Tit_Tributaria_Categoria { get; set; }
        public String Tit_Tributaria_Id { get; set; }
        public long Cant_Cotitulares { get; set; }
        public String Rp_Identificacion { get; set; }
        public String Rp_Nombre { get; set; }
        public String Rp_Tributaria_Categoria { get; set; }
        public String Rp_Tributaria_Id { get; set; }
        public String Inscripcion { get; set; }
        public String Restricciones { get; set; }
        public String Dpost_Pais { get; set; }
        public String Dpost_Provincia { get; set; }
        public String Dpost_Depto { get; set; }
        public String Dpost_Codigo_Postal { get; set; }
        public String Dpost_Localidad { get; set; }
        public String Dpost_Barrio { get; set; }
        public String Dpost_Calle { get; set; }
        public int Dpost_Numero { get; set; }
        public String Dpost_Piso { get; set; }
        public String Dpost_Departamento { get; set; }
        public String Dpost_Local { get; set; }
        public String Dprop_Pais { get; set; }
        public String Dprop_Provincia { get; set; }
        public String Dprop_Depto { get; set; }
        public String Dprop_Codigo_Postal { get; set; }
        public String Dprop_Localidad { get; set; }
        public String Dprop_Barrio { get; set; }
        public String Dprop_Calle { get; set; }
        public int Dprop_Numero { get; set; }
        public String Dprop_Piso { get; set; }
        public String Dprop_Departamento { get; set; }
        public String Dprop_Local { get; set; }
        public long Coeficiente_Fiscal { get; set; }
        public long Coeficiente_Tierra { get; set; }
        public long Coeficiente_Mejoras { get; set; }
        public int Subdivision_AAAA { get; set; }
        public long Subdivision_Cta { get; set; }
        public String Subdivision_Lib { get; set; }
        public long Subdivision_Folio { get; set; }
        public String Zona_Geografica { get; set; }
        public long Coeficiente_Ph { get; set; }
        public String Tramites { get; set; }
        public String Dest_Recibo { get; set; }
        public String Expediente_Constr { get; set; }
        public String Expediente_En_Tramite { get; set; }
        public String Designacion { get; set; }
        public String Lugar { get; set; }
        public String Caract { get; set; }
        public String Rubro { get; set; }
        public String Destino { get; set; }
        public int Certf_Info_Cat_Urb { get; set; }
        public int Certf_Anio { get; set; }
        public String Empadronamiento { get; set; }
        public int Plantas { get; set; }
        public int Subsuelos { get; set; }
        public String Zona_Curb { get; set; }
        public String Es_Esquina { get; set; }
        public String Matricula { get; set; }
        public String Tomo { get; set; }
        public String Folio { get; set; }
        public long Anio_Matricula { get; set; }
        public String Finca { get; set; }
        public String Zona_Codigo { get; set; }
        public String Zona_Descripcion{ get; set; }
        public string Porc_Ph { get; set; }
        public string Sup_Tierra { get; set; }
        public string Sup_Vivienda { get; set; }
        public long Sup_Inst_Obras { get; set; }
        public long Sup_Industria{ get; set; }
        public double Sup_Negocio { get; set; }
        public string Val_Tierra{ get; set; }
        public long Val_Vivienda{ get; set; }
        public long Val_Inst_Obras { get; set; }
        public long Val_Industria{ get; set; }
        public long Val_Negocio { get; set; }
        public String Serv_Viv_Agua { get; set; }
        public String Serv_Viv_Cloaca{ get; set; }
        public long Serv_Viv_Recoleccion { get; set; }
        public long Serv_Viv_Conservacion { get; set; }
        public long Serv_Viv_Barrido { get; set; }
        public int Can_Viviendas { get; set; }
        public String Serv_Com_Agua { get; set; }
        public String Serv_Com_Cloaca { get; set; }
        public long Serv_Com_Recoleccion { get; set; }
        public long Serv_Com_Conservacion { get; set; }
        public long Serv_Com_Barrido { get; set; }
        public int Unidad_Comercios { get; set; }
    }
}
