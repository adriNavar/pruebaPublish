using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class Nomenclatura : IEntity
    {
        public long NomenclaturaID { get; set; }
        public long ParcelaID { get; set; }
        public string Nombre { get; set; }
        public long TipoNomenclaturaID { get; set; }
        public long? UsuarioAltaID { get; set; }
        public DateTime? FechaAlta { get; set; }
        public long? UsuarioModificacionID { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public long? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }

        /* PROPIEDADES DE NAVEGACION */
        public TipoNomenclatura Tipo { get; set; }
        [JsonIgnore]
        public Parcela Parcela { get; set; }

        public Nomenclatura()
        {
        }

        public Dictionary<string, string> GetNomenclaturas()
        {
            Dictionary<string, string> retVal = null;
            try
            {
                retVal = new Dictionary<string, string>();
                Regex regex = new Regex(this.Tipo.ExpresionRegular);
                List<string> nombres = regex.GetGroupNames().ToList();
                GroupCollection partes = regex.Match(this.Nombre).Groups;
                nombres.RemoveAt(0);
                
                for (int i = 0; i < nombres.Count; i++)
                {
                    retVal.Add(nombres[i], partes[i + 1].Value);
                }

            }
            catch (Exception ex)
            {
                //ver catcheo
                throw ex;
            }
            return retVal;
        }
    }
}
