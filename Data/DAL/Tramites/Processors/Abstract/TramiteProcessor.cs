using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.ValidacionesDB;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Enums;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Configurators;
using GeoSit.Data.DAL.Tramites.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoSit.Data.DAL.Tramites.Processors.Abstract
{
    abstract class TramiteProcessor : IProcessor
    {
        protected readonly int _tipoTramite;
        protected readonly METramite _tramite;
        protected readonly GeoSITMContext _contexto;

        protected readonly List<IAction> _acciones;
        protected readonly List<IAction> _accionesInformes;
        public TramiteProcessor(METramite tramite, GeoSITMContext context)
            : this(tramite, context, 0) { }
        protected TramiteProcessor(METramite tramite, GeoSITMContext contexto, int tipoTramite)
        {
            _tramite = tramite;
            _contexto = contexto;
            _tipoTramite = tipoTramite;
            _acciones = new List<IAction>();
            _accionesInformes = new List<IAction>();
        }

        public void Configure()
        {
            ConfigurePreProcessingActions();
            ConfiguratorTramiteBuilder.GetConfigurator(_tipoTramite, _tramite, _contexto).Configure(this);
            ConfigurePostProcessingActions();
        }
        public virtual void Configure(IEnumerable<IAction> actions)
        {
            foreach (var action in actions)
            {
                _acciones.Add(action);
            }
        }
        public virtual bool IsTipoTramite()
        {
            return _tipoTramite == _tramite.IdTipoTramite;
        }
        public virtual void Process()
        {
            ProcesarAcciones();
            ProcesarInformes();
        }
        protected void ProcesarAcciones()
        {
            using (var trans = _contexto.Database.BeginTransaction())
            {
                try
                {
                    try
                    {
                        bool hayError = false;
                        int idx = 0;
                        while (!hayError && idx < _acciones.Count)
                        {
                            hayError = !_acciones[idx++].Execute();
                        }
                        if (hayError)
                        {
                            var accion = _acciones[--idx];
                            throw new ValidacionException(accion.Resultado, accion.Errores);
                        }
                        _contexto.SaveChanges();
                        trans.Commit();
                    }
                    catch (ValidacionException ex)
                    {
                        _contexto.GetLogger().LogInfo($"Procesamiento de Trámite {_tramite.IdTramite}:\n\t{string.Join("\n\t", ex.Errores)}");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _contexto.GetLogger().LogError($"{this.GetType().Name}({_tramite.IdTramite},{_tipoTramite})", ex);
                        throw;
                    }
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
        protected void ProcesarInformes()
        {
            try
            {
                var errores = _accionesInformes.Where(ai => !ai.Execute()).SelectMany(ai => ai.Errores);
                if (errores.Any())
                {
                    throw new ValidacionException(ResultadoValidacion.Advertencia, errores);
                }
                _contexto.SaveChanges();
            }
            catch (ValidacionException ex)
            {
                _contexto.GetLogger().LogInfo($"Procesamiento de Trámite {_tramite.IdTramite} - Generación de Informes:\n\t{string.Join("\n\t", ex.Errores)}");
                throw;
            }
            catch (Exception ex)
            {
                _contexto.GetLogger().LogError($"Generación de Informes - {this.GetType().Name}({_tramite.IdTramite},{_tipoTramite})", ex);
                throw;
            }
        }
        abstract protected void ConfigurePreProcessingActions();
        abstract protected void ConfigurePostProcessingActions();
    }
}