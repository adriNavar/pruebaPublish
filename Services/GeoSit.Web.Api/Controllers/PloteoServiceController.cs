using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.BusinessEntities.Ploteo;
using GeoSit.Web.Api.Ploteo;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Web.Api.Controllers
{
    public class PloteoServiceController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public PloteoServiceController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [ResponseType(typeof(List<Coleccion>))]
        public IHttpActionResult GetColeccionesByComponentePrincipal(long idComponentePrincipal)
        {
            List<Coleccion> colecciones = _unitOfWork.ColeccionRepository.GetColecciones().ToList();
            List<Coleccion> coleccionesComponentePrincipal = new List<Coleccion>();

            foreach (var coleccion in colecciones)
            {
                _unitOfWork.ColeccionRepository.GetComponentes(coleccion);
                if (coleccion.Componentes.Any(c => c.ComponenteId == idComponentePrincipal))
                    coleccionesComponentePrincipal.Add(coleccion);
            }

            return Ok(coleccionesComponentePrincipal);
        }

        [HttpGet]
        [ResponseType(typeof(List<Coleccion>))]
        public IHttpActionResult GetColeccionByCurrentUser(long UsuarioId)
        {
            var colecciones = _unitOfWork.ColeccionRepository.GetColeccionesByUserId(UsuarioId);

            return Ok(colecciones);
        }

        [HttpGet]
        [ResponseType(typeof(List<Coleccion>))]
        public IHttpActionResult GetColeccionByCurrentUserColeccionA4(long UsuarioId)
        {
            ComponenteController componenteController = new ComponenteController(_unitOfWork);
            var componentesPloteables = componenteController.GetComponentesPloteables();

            var colecciones = _unitOfWork.ColeccionRepository.GetColeccionesUsuarioByComponentesPrincipales(UsuarioId, componentesPloteables.Select(c => c.ComponenteId).ToArray()).ToList();

            return Ok(colecciones);
        }

        [HttpGet]
        [ResponseType(typeof(List<ImagenSatelital>))]
        public IHttpActionResult GetAllImagenSatelital()
        {
            var lstImagenSatetlital = _unitOfWork.ImagenSatelitalRepository.GetAllImagenSatelital().ToList();
            return Ok(lstImagenSatetlital);
        }

        [HttpGet]
        [ResponseType(typeof(Coleccion))]
        public IHttpActionResult GetColeccionById(long idColeccion)
        {
            var coleccion = _unitOfWork.ColeccionRepository.GetColeccionById(idColeccion);

            return Ok(coleccion);
        }

        [HttpPost]
        [ResponseType(typeof(List<KeyValuePair<long, long>>))]
        public IHttpActionResult GetManzanasByIds(string[] listaIdsApic, bool esApic)
        {
            //VIENE DE GetManzanasByIds, PLOTEOCONTROLLER
            //LINEA 1358
            bool finLoop = false;
            int idsMaximos = 20;
            int step = 0;
            List<string> lstIds = listaIdsApic.ToList();
            List<KeyValuePair<long, long>> resultado = new List<KeyValuePair<long, long>>();

            //SE CREA UNA LISTA DE PARES (IDPARCELA, IDMANZANA)
            //QUE TENDRA COMO MAXIMO 20 ELEMENTOS
            while (!finLoop)
            {
                var listado = new List<string>();
                if (listaIdsApic.Length > (step + idsMaximos))
                {
                    listado = lstIds.GetRange(step, idsMaximos);
                    step += idsMaximos;
                }
                else
                {
                    listado = lstIds.GetRange(step, lstIds.Count - step);
                    step = lstIds.Count;
                }
                finLoop = step == lstIds.Count;

                resultado.AddRange(_unitOfWork.ParcelaPlotRepository.GetManzanas(listado, esApic));
            }

            //DEVUELVE UNA LISTA DE PARCELAS AGRUPADAS POR MANZANA.
            //CADA PAR ES (IDPARCELA, IDMANZANA)
            return Ok(resultado);
        }

        [HttpGet]
        [ResponseType(typeof(ObjetoPloteable))]
        public IHttpActionResult GetObjetoPloteable(int idObjeto, string componente)
        {
            ObjetoPloteableHelper helper = new ObjetoPloteableHelper(_unitOfWork);

            ObjetoPloteable objeto = helper.GetObjetoPloteable(idObjeto, componente);
            if (objeto != null)
            {
                objeto.componente = componente;
            }

            return Ok(objeto);
        }

        //[HttpPost]
        //[ResponseType(typeof(List<KeyValuePair<long, long>>))]
        //public IHttpActionResult GetManzanasByParcelasAPIC_ID(string[] listaParcelaApicIdsApic)
        //{
        //    bool finLoop = false;
        //    int idsMaximos = 20;
        //    int step = 0;
        //    List<string> lala = listaParcelaApicIdsApic.ToList();
        //    List<KeyValuePair<long, long>> resultado = new List<KeyValuePair<long, long>>();

        //    //SE CREA UNA LISTA DE PARES (IDPARCELA, IDMANZANA)
        //    //QUE TENDRA COMO MAXIMO 20 ELEMENTOS
        //    while (!finLoop)
        //    {
        //        var listado = new List<string>();
        //        if (listaParcelaApicIdsApic.Length > (step + idsMaximos))
        //        {
        //            listado = lala.GetRange(step, idsMaximos);
        //            step += idsMaximos;
        //        }
        //        else
        //        {
        //            listado = lala.GetRange(step, lala.Count - step);
        //            step = lala.Count;
        //        }
        //        finLoop = step == lala.Count;

        //        resultado.AddRange(_unitOfWork.ParcelaPlotRepository.GetManzanasByParcelasAPIC_ID(listado));
        //    }

        //    //DEVUELVE UNA LISTA DE PARCELAS AGRUPADAS POR MANZANA.
        //    //CADA PAR ES (IDPARCELA, IDMANZANA)
        //    return Ok(resultado);
        //}


        //[HttpPost]
        //[ResponseType(typeof(List<string>))]
        //public IHttpActionResult GetManzanasByLisManzanasAPICIDs(string[] listaIdsApic)
        //{
        //    var resultado = new List<string>();
        //    resultado = _unitOfWork.ParcelaPlotRepository.GetManzanasByLisManzanasAPICIDs(listaIdsApic.ToList()).ToList();
        //    return Ok(resultado);
        //}

        //[HttpPost]
        //[ResponseType(typeof(List<string>))]
        //public IHttpActionResult GetLisManzanasAPICIDsByManzanasID(string[] listaIdsManzana)
        //{
        //    //VIENE DE LINEA 688, PLOTEOCONTROLLER
        //    var resultado = new List<string>();
        //    resultado = _unitOfWork.ParcelaPlotRepository.GetLisManzanasAPICIDsByManzanasID(listaIdsManzana.ToList()).ToList();
        //    return Ok(resultado);
        //}


        ////
        //[HttpPost]
        //[ResponseType(typeof(List<string>))]
        //public IHttpActionResult GetParcela_APICID_ByParcelaID(string[] listaIdsParcela)
        //{
        //    //VIENE DE LINEA 688, PLOTEOCONTROLLER
        //    var resultado = new List<string>();
        //    //Comentado por RA
        //    //resultado = _unitOfWork.ParcelaPlotRepository.GetLisManzanasAPICIDsByManzanasID(listaIdsParcela.ToList()).ToList();
        //    resultado = _unitOfWork.ParcelaPlotRepository.GetParcela_APICID_ByParcelaID(listaIdsParcela.ToList()).ToList();
        //    return Ok(resultado);
        //}
    }
}