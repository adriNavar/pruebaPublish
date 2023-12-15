using GeoSit.Data.BusinessEntities.Designaciones;
using GeoSit.Data.BusinessEntities.Inmuebles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoSit.Client.Web.Models
{
    public class DesignacionParcelaModel
    {
        public Designacion Designacion { get; set; }
        public string Descripcion { get; private set; }

        public DesignacionParcelaModel(Designacion designacion, Parcela parcela)
        {
            Designacion = designacion;
            Descripcion = formatDescripcion(designacion, parcela.TipoParcelaID);
        }

        private string formatDescripcion(Designacion designacion, long idTipoParcela)
        {
            var partes = new Dictionary<int, Tuple<string, string>>()
            {
                {1, new Tuple<string, string>("Dep.", designacion.Departamento)},
                {2, new Tuple<string, string>("Loc.", designacion.Localidad)}
            };
            if (idTipoParcela == 2 && idTipoParcela == 3) //2 y 3 es rural y subrural
            {
                if (!string.IsNullOrEmpty(designacion.Paraje?.Trim()))
                {
                    partes.Add(3, new Tuple<string, string>("Paraje", designacion.Paraje));
                }
                if (!string.IsNullOrEmpty(designacion.Seccion?.Trim()))
                {
                    partes.Add(4, new Tuple<string, string>("Sección", designacion.Seccion));
                }
                if (!string.IsNullOrEmpty(designacion.Quinta?.Trim()))
                {
                    partes.Add(10, new Tuple<string, string>("Quinta", designacion.Quinta));
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(designacion.Calle?.Trim()))
                {
                    partes.Add(3, new Tuple<string, string>("Calle", designacion.Calle));
                }
                if (!string.IsNullOrEmpty(designacion.Numero?.Trim()))
                {
                    partes.Add(4, new Tuple<string, string>("Nro.", designacion.Numero));
                }
                if (!string.IsNullOrEmpty(designacion.Manzana?.Trim()))
                {
                    partes.Add(20, new Tuple<string, string>("Mza.", designacion.Manzana));
                }
            }
            if (!string.IsNullOrEmpty(designacion.Chacra?.Trim()))
            {
                partes.Add(8, new Tuple<string, string>("Chacra", designacion.Chacra));
            }
            if (!string.IsNullOrEmpty(designacion.Fraccion?.Trim()))
            {
                partes.Add(15, new Tuple<string, string>("Fracción", designacion.Fraccion));
            }
            if (!string.IsNullOrEmpty(designacion.Lote?.Trim()))
            {
                partes.Add(25, new Tuple<string, string>("Parcela", designacion.Lote));
            }
            return string.Join(" - ", partes.OrderBy(e => e.Key).Select(e => $"{e.Value.Item1}: {e.Value.Item2}"));
        }
    }
}