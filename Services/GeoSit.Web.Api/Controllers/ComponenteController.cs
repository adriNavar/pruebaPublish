using System.Web.Http;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.DAL;
using System.Web.Http.Description;
using System.Collections.Generic;
using System.Linq;
using System;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Web.Api.Controllers
{
    public class ComponenteController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public ComponenteController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get()
        {
            var componentes = _unitOfWork.ComponenteRepository.GetComponentes();
            return Ok(componentes);
        }

        public IEnumerable<Componente> ObtenerComponentes()
        {
            IEnumerable<Componente> componentes = _unitOfWork.ComponenteRepository.GetComponentes();
            return componentes;
        }

        public IHttpActionResult Get(int id)
        {
            var componente = _unitOfWork.ComponenteRepository.GetComponenteById(id);
            return Ok(componente);
        }

        public IHttpActionResult Get(string docType)
        {
            var componente = _unitOfWork.ComponenteRepository.GetComponenteByDocType(docType);
            return Ok(componente);
        }

        public IHttpActionResult GetByLayer(string layer)
        {
            var componente = _unitOfWork.ComponenteRepository.GetComponenteByLayer(layer);
            return Ok(componente);
        }

        public IHttpActionResult GetByTable(string table)
        {
            var componente = _unitOfWork.ComponenteRepository.GetComponenteByTable(table);
            return Ok(componente);
        }

        public IHttpActionResult Post(Componente componente)
        {
            _unitOfWork.ComponenteRepository.InsertComponente(componente);
            _unitOfWork.Save();
            return Ok();
        }

        public IHttpActionResult Delete(int id)
        {
            _unitOfWork.ComponenteRepository.DeleteComponente(id);
            _unitOfWork.Save();
            return Ok();
        }


        public List<Componente> GetComponentesPloteables()
        {
            using (var db = GeoSITMContext.CreateContext())
            {

                List<Componente> componentesDB = db.Componente.ToList();
                List<Jerarquia> jerarquiaDB = db.Jerarquia.ToList();

                long parcela = Convert.ToInt64(_unitOfWork.ParametroRepository.GetParametroByDescripcion("ID_COMPONENTE_PARCELA"));
                long manzana = Convert.ToInt64(_unitOfWork.ParametroRepository.GetParametroByDescripcion("ID_COMPONENTE_MANZANA"));
                var componenteParcela = db.Componente.Find(parcela);
                var componenteManzana = db.Componente.Find(manzana);

                //Componentes parcela y manzana
                List<Componente> componentesPloteables = new List<Componente>() { componenteManzana, componenteParcela };

                //Componentes de grafico 3
                componentesPloteables.AddRange(db.Componente.Where(c => c.Graficos == 3));

                if (componenteParcela != null)
                {

                    List<long> componentesAsociados = jerarquiaDB.Where(j => j.ComponenteInferiorId != parcela && j.ComponenteInferiorId != manzana).Select(j => j.ComponenteInferiorId).Distinct().ToList();

                    var componentesAAnalizar = componentesDB.Where(c => componentesAsociados.Any(ca => ca == c.ComponenteId)).ToList();
                    componentesAAnalizar.RemoveAll(c => componentesPloteables.Any(cp => cp.ComponenteId == c.ComponenteId));

                    foreach (Componente componente in componentesAAnalizar)
                    {

                        List<Componente> componentes = new List<Componente>() { componente };

                        //Busco si el componente tiene una asociación hacia arriba con parcelas
                        Componente componenteInferior = componente;
                        Jerarquia asociacion = null;

                        do
                        {
                            asociacion = jerarquiaDB.FirstOrDefault(j => j.ComponenteInferiorId == componenteInferior.ComponenteId);
                            if (asociacion != null)
                            {
                                Componente componenteSuperior = componentesDB.FirstOrDefault(c => c.ComponenteId == asociacion.ComponenteSuperiorId);
                                componentes.Add(componenteSuperior);
                                componenteInferior = componenteSuperior;
                            }


                        } while (asociacion != null && componenteInferior != null && componenteInferior.ComponenteId != parcela);


                        //Si tiene asociación con parcelas, es ploteable
                        if (componentes.Any(c => c.ComponenteId == parcela))
                        {
                            componentesPloteables.Add(componente);
                        }

                    }
                }

                return componentesPloteables;

            }
        }
        /// <summary>
        /// Obtiene los componentes para el formulario consultas frecuentes.
        /// </summary>
        /// <param name="idServicio"> 1 = Agua, 2 = Cloaca</param>
        ///// <returns>Devuelve una lisca de componentes con el servicio correspondiente o null</returns>
        public List<Componente> ObtenerComponentesFrecuentes(int idServicio)
        {
            using (ORAGeoSIT db = new ORAGeoSIT())
            {
                List<Componente> componentesConFre = new List<Componente>() { };
                //componentesConFre = db.Componente.Where(d => d.EsConFre == 1 && (d.IdServicio == idServicio || d.IdServicio == null)).ToList();
                componentesConFre = db.Componente.Where(d => d.EsConFre == 1).ToList();
                return componentesConFre;
            }
        }

        public Componente GetComponenteById(int id)
        {
            using (ORAGeoSIT db = new ORAGeoSIT())
            {
                var componente = _unitOfWork.ComponenteRepository.GetComponenteById(id);
                return componente;
            }
        }

        [HttpGet]
        [ResponseType(typeof(List<Componente>))]
        public IHttpActionResult GetPloteables()
        {
            return Ok(GetComponentesPloteables());
        }

        [HttpGet]
        [ResponseType(typeof(List<Componente>))]
        public IHttpActionResult ObtenerComponentesFrecuentesWeb(int idServicio)
        {

            return Ok(ObtenerComponentesFrecuentes(idServicio));
        }
    }
}
