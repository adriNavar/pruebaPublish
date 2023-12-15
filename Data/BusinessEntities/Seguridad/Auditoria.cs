using System;
using GeoSit.Data.BusinessEntities.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections;
using GeoSit.Data.BusinessEntities.MesaEntradas;

namespace GeoSit.Data.BusinessEntities.Seguridad
{
    public class Auditoria : IEntity
    {
        public long Id_Auditoria { get; set; }
        public long Id_Usuario { get; set; }
        public long Id_Evento { get; set; }
        public string Datos_Adicionales { get; set; }
        public DateTime Fecha { get; set; }
        public string Ip { get; set; }
        public string Machine_Name { get; set; }
        public string Autorizado { get; set; }
        public string Objeto_Origen { get; set; }
        public string Objeto_Modif { get; set; }
        public long Id_Tipo_Operacion { get; set; }
        public string Objeto { get; set; }
        public long Id_Objeto { get; set; }
        public long Cantidad { get; set; }
        public int? Id_Tramite { get; set; }

        //PROPIEDADES DE NAVEGACIÓN

        public METramite tramites { get; set; }

        public Usuarios usuarios { get; set; }

        public SEEvento eventos { get; set; }


        public Auditoria(long usuario, string evento, string datosAdicionales, string machineName, string ip, string autorizado,
                    object objetosOrigen, object objetosModificados, string objetoNombre, long cantidad, string tipoOperacion)
        {
            Id_Usuario = usuario;
            Id_Evento = Convert.ToInt64(evento);
            Datos_Adicionales = datosAdicionales;
            Fecha = DateTime.Now;
            Ip = ip;
            Machine_Name = machineName;
            Autorizado = autorizado;
            Objeto_Origen = JsonConvert.SerializeObject(objetosOrigen ?? "", new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            Objeto_Modif = JsonConvert.SerializeObject(objetosModificados ?? "",new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            Objeto = objetoNombre;
            Cantidad = cantidad;
            Id_Tipo_Operacion = Convert.ToInt64(tipoOperacion);
        }


        public Auditoria()
        {
        }
    }
}


