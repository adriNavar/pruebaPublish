using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.Ploteo;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Web.Api.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoSit.Web.Api.Ploteo
{
    public class ObjetoPloteableHelper
    {

        private readonly GeoSITMContext db = GeoSITMContext.CreateContext();
        UnitOfWork _unitOfWork;

        public ObjetoPloteableHelper(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ObjetoPloteable GetObjetoPloteable(long idObjeto, string docType)
        {
            Componente componente = db.Componente.FirstOrDefault(c => c.DocType.Equals(docType));
            long componenteParcela = Convert.ToInt64(_unitOfWork.ParametroRepository.GetParametroByDescripcion("ID_COMPONENTE_PARCELA"));
            long componenteManzana = Convert.ToInt64(_unitOfWork.ParametroRepository.GetParametroByDescripcion("ID_COMPONENTE_MANZANA"));

            ObjetoPloteable objeto = null;
            
            if (componente != null)
            {
                //Si el componente es manzana, devuelvo un objeto con su misma id
                if(componente.ComponenteId == componenteManzana)
                {
                    objeto = new ObjetoPloteable();
                    objeto.idObjeto = idObjeto;
                    objeto.idManzana = idObjeto;
                }
                else if (componente.ComponenteId == componenteParcela) //Si el componente es parcela, obtengo el idManzana
                {
                    long? idManzana = GetManzanaByParcela(idObjeto);
                    if (idManzana != null) 
                    {
                        objeto = new ObjetoPloteable();
                        objeto.idObjeto = idObjeto;
                        objeto.idParcela = idObjeto.ToString();
                        objeto.idManzana = (long)idManzana;
                    }
                }
                else if(componente.Graficos == 3) //Si son puntos, realizo la asociación por aproximación
                {

                    long? idParcela = GetParcelaByDistance(componente, idObjeto);
                    if (idParcela != null)
                    {
                        long? idManzana = GetManzanaByParcela((long)idParcela);
                        if (idManzana != null)
                        {
                            objeto = new ObjetoPloteable();
                            objeto.idObjeto = idObjeto;
                            objeto.idParcela = idParcela.ToString();
                            objeto.idManzana = (long)idManzana;
                        }
                    }
                }
                else //Sino, intento buscar la parcela asociada
                {
                    long? idParcela = GetParcela(componente,idObjeto);
                    if (idParcela != null)
                    {
                        long? idManzana = GetManzanaByParcela((long)idParcela);
                        if (idManzana != null)
                        {
                            objeto = new ObjetoPloteable();
                            objeto.idObjeto = idObjeto;
                            objeto.idParcela = idParcela.ToString();
                            objeto.idManzana = (long)idManzana;
                        }
                    }
                }

            }

            /*if(objeto != null)
            {
                var manzana = db.Manzanas.FirstOrDefault(a => a.FeatId == objeto.idManzana);
                if (manzana != null)
                {
                    objeto.apicIdManzana = manzana.ApicId;
                }

                if(objeto.idParcela != null)
                {
                    long idParcela = long.Parse(objeto.idParcela);
                    var parcela = db.Parcelas.FirstOrDefault(a => a.ParcelaID == idParcela);
                    if (parcela != null)
                    {
                        objeto.apicIdParcela = parcela.ApicId;
                    }

                }

            }*/
            
            return objeto;

        }

        private List<Componente> GetComponentesPloteables()
        {
            ComponenteController controller = new ComponenteController(_unitOfWork);
            return controller.GetComponentesPloteables();
        }


        private List<Jerarquia> GetJerarquiaHastaParcela(Componente componente) {

            long parcela = Convert.ToInt64(_unitOfWork.ParametroRepository.GetParametroByDescripcion("ID_COMPONENTE_PARCELA"));

            List<Componente> componentesDB = db.Componente.ToList();
            List<Jerarquia> jerarquiaDB = db.Jerarquia.ToList();

            List<Jerarquia> jerarquia = new List<Jerarquia>();
            
            var componenteParcela = componentesDB.FirstOrDefault(c => c.ComponenteId == parcela);
            Componente componenteInferior = componente;

            Jerarquia asociacion = null;

            do
            {
                asociacion = jerarquiaDB.FirstOrDefault(j => j.ComponenteInferiorId == componenteInferior.ComponenteId);
                if (asociacion != null)
                {
                    Componente componenteSuperior = componentesDB.SingleOrDefault(c => c.ComponenteId == asociacion.ComponenteSuperiorId);
                    jerarquia.Add(asociacion);
                    componenteInferior = componenteSuperior;
                }


            } while (asociacion != null && componenteInferior != null && componenteInferior.ComponenteId != parcela);

            jerarquia.Reverse();

            return jerarquia;
        }

        private long? GetParcela(Componente componente, long idObjeto)
        {
            long parcela = Convert.ToInt64(_unitOfWork.ParametroRepository.GetParametroByDescripcion("ID_COMPONENTE_PARCELA"));

            long? idParcela = null;
            
            var componenteParcela = db.Componente.FirstOrDefault(c => c.ComponenteId == parcela);
            
            List<Componente> componentesDB = db.Componente.ToList();

            List<Componente> componentesPloteables = GetComponentesPloteables();

            //Si tiene asociación con parcelas, armo la query para obtener el id_parcela
            if (componentesPloteables.Any(c => c.ComponenteId == componente.ComponenteId))
            {

                var claveParcela = db.Atributo.First(a => a.ComponenteId == componenteParcela.ComponenteId && a.EsClave == 1);
                var query = "select " + componenteParcela.Esquema + "." + componenteParcela.Tabla + "." + claveParcela.Campo + " from " + componenteParcela.Esquema + "." + componenteParcela.Tabla;
                
                foreach (Jerarquia asociacion in GetJerarquiaHastaParcela(componente))
                {
                    Atributo atributoInferior = db.Atributo.First(a => a.AtributoId == asociacion.AtributoInferiorId);
                    Atributo atributoSuperior = db.Atributo.First(a => a.AtributoId == asociacion.AtributoSuperiorId);

                    Componente componenteInferior = componentesDB.First(c => c.ComponenteId == asociacion.ComponenteInferiorId);
                    Componente componenteSuperior = componentesDB.First(c => c.ComponenteId == asociacion.ComponenteSuperiorId);
                    query += " inner join " + componenteInferior.Esquema + "." + componenteInferior.Tabla + " on " + componenteInferior.Esquema + "." + componenteInferior.Tabla + "." + atributoInferior.Campo + " = " + componenteSuperior.Esquema + "." + componenteSuperior.Tabla + "." + atributoSuperior.Campo;

                    componenteInferior = componenteSuperior;
                }

                var clave = db.Atributo.First(a => a.ComponenteId == componente.ComponenteId && a.EsClave == 1);
                query += " where " + componente.Esquema + "." + componente.Tabla + "." + clave.Campo + " = " + idObjeto;

                idParcela = db.Database.SqlQuery<long>(query).FirstOrDefault();

                idParcela = idParcela != 0 ? idParcela : null;

            }

            return idParcela;
        }
        

        private long? GetManzanaByParcela(long idParcela)
        {
            return _unitOfWork.ParcelaPlotRepository.GetIdManzanaByParcela(idParcela);
        }
        
        private long? GetParcelaByDistance(Componente componenteAPlotear, long idObjeto)
        {
            long parcela = Convert.ToInt64(_unitOfWork.ParametroRepository.GetParametroByDescripcion("ID_COMPONENTE_PARCELA"));
            var componenteObjetivo = db.Componente.Find(parcela);

            var atributoSegmento = db.Atributo.FirstOrDefault(a => a.AtributoId == 41 && a.ComponenteId == parcela);

            long? idObjetivo = null;

            try
            {

                var claveObjetivo = db.Atributo.First(a => a.ComponenteId == componenteObjetivo.ComponenteId && a.EsClave == 1);
                var geometryObjetivo = db.Atributo.First(a => a.ComponenteId == componenteObjetivo.ComponenteId && a.EsGeometry);

                var claveComponente = db.Atributo.First(a => a.ComponenteId == componenteAPlotear.ComponenteId && a.EsClave == 1);
                var geometryComponente = db.Atributo.First(a => a.ComponenteId == componenteAPlotear.ComponenteId && a.EsGeometry);

                var query = "select " + componenteObjetivo.Esquema + "." + componenteObjetivo.Tabla + "." + claveObjetivo.Campo + " from " + componenteObjetivo.Esquema + "." + componenteObjetivo.Tabla;

                query += " where SDO_NN(" + componenteObjetivo.Esquema + "." + componenteObjetivo.Tabla + "." + geometryObjetivo.Campo + ", (select " + componenteAPlotear.Esquema + "." + componenteAPlotear.Tabla + "." + geometryComponente.Campo + " from " + componenteAPlotear.Esquema + "." + componenteAPlotear.Tabla + " where " + componenteAPlotear.Esquema + "." + componenteAPlotear.Tabla + "." + claveComponente.Campo + " = " + idObjeto + "),'sdo_batch_size=10') = 'TRUE' AND ROWNUM <= 1";

                //Me aseguro que pertenezca a un segmento
                if (atributoSegmento != null) {
                    query += " and " + componenteObjetivo.Esquema + "." + componenteObjetivo.Tabla + "." + atributoSegmento.Campo + " is not null";
                }

                idObjetivo = db.Database.SqlQuery<long>(query).FirstOrDefault();


            }
            catch (Exception e)
            {

            }

            return idObjetivo;
        }
        

    }
}