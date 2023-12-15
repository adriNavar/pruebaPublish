using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace GeoSit.Data.DAL.Repositories
{
    public class ColeccionRepository : IColeccionRepository
    {
        private readonly GeoSITMContext _context;

        public ColeccionRepository(GeoSITMContext context)
        {
            _context = context;
        }
        public IEnumerable<Coleccion> GetColecciones()
        {
            return _context.Coleccion;
        }

        public IEnumerable<Coleccion> GetColeccionesByUserId(long usuarioId)
        {
            return _context.Coleccion.Where(c => c.UsuarioAlta == usuarioId && c.FechaBaja == null);
        }

        public Coleccion GetColeccionById(long coleccionId)
        {
            var coleccion = _context.Coleccion.FirstOrDefault(c => c.ColeccionId == coleccionId && c.FechaBaja == null);
            if (coleccion != null)
                GetComponentes(coleccion);
            return coleccion;
        }

        public void GetComponentes(Coleccion coleccion)
        {
            _context.Entry(coleccion).Collection(c => c.Componentes).Query().Where(c => c.FechaBaja == null).Load();
        }
        public void GetAtributos(Componente componente)
        {
            _context.Entry(componente).Collection(c => c.Atributos).Load();
        }

        public void NuevaColeccion(Coleccion coleccion)
        {
            _context.Entry(coleccion).State = coleccion.ColeccionId == 0 ? EntityState.Added : EntityState.Modified;
            //_context.Coleccion.Add(coleccion);


            _context.SaveChanges();
        }

        public void AgregarComponentesColeccion(Coleccion coleccionOrigen, Coleccion coleccionDestino, bool validarDuplicados)
        {
            _context.Configuration.ValidateOnSaveEnabled = false;
            var lista = new List<ColeccionComponente>();
            foreach (var item in coleccionOrigen.Componentes.Where(c => c.FechaBaja == null))
            {
                if (validarDuplicados)
                {
                    if (coleccionDestino != null && coleccionDestino.Componentes != null && coleccionDestino.Componentes.Any(c => c.ComponenteId == item.ComponenteId && c.ObjetoId == item.ObjetoId))
                    {
                        continue;
                    }
                }
                lista.Add(new ColeccionComponente
                {
                    ColeccionId = coleccionDestino.ColeccionId,
                    ComponenteId = item.ComponenteId,
                    ObjetoId = item.ObjetoId,
                    UsuarioAlta = coleccionDestino.UsuarioAlta,
                    FechaAlta = DateTime.Now
                });
            }
            if (lista.Any())
            {
                _context.ColeccionComponente.AddRange(lista);
                _context.Configuration.ValidateOnSaveEnabled = true;
                _context.SaveChanges();
            }
        }

        public bool ValidarNombreColeccion(long usuarioId, string nombreColeccion)
        {
            return _context.Coleccion.Any(c => c.UsuarioAlta == usuarioId && c.Nombre == nombreColeccion && c.FechaBaja == null);
        }

        public Atributo GetAtributosById(long idAtributo)
        {
            Atributo atributo = _context.Atributo.FirstOrDefault(a => a.AtributoId == idAtributo);
            if (atributo == null)
            {
                return null;
            }
            return atributo;
        }

        public List<Componente> GetComponentesRelacionados(long idComponente)
        {
            var componentes = new List<Componente>();
            componentes.AddRange(ObtenerComponentesPadres(idComponente));
            componentes.AddRange(new List<Componente>(_context.Componente.Where(a => a.ComponenteId == idComponente)));
            componentes.AddRange(ObtenerComponentesHijos(idComponente));
            foreach (var componente in componentes)
            {
                GetAtributos(componente);
            }

            return componentes;
        }

        private List<Componente> ObtenerComponentesHijos(long idComponente)
        {

            var componentes = new List<Componente>();

            var arbolJerarquiasPlano = ObtenerJerarquiasHijas(idComponente);
            foreach (var item in arbolJerarquiasPlano)
            {
                componentes.AddRange(_context.Componente.Where(a => a.ComponenteId == item.ComponenteInferiorId));
            }

            return componentes;
        }

        private List<Componente> ObtenerComponentesPadres(long idComponente)
        {
            var componentes = new List<Componente>();
            var arbolJerarquiasPlano = ObtenerJerarquiasPadres(idComponente);
            foreach (var item in arbolJerarquiasPlano)
            {
                componentes.AddRange(_context.Componente.Where(a => a.ComponenteId == item.ComponenteSuperiorId));
            }

            return componentes;
        }

        public List<Jerarquia> ObtenerJerarquiasHijas(long idJerarquia)
        {
            var jerarquias = new List<Jerarquia>(_context.Jerarquia.Where(a => a.ComponenteSuperiorId == idJerarquia));
            var jerarquiashijas = new List<Jerarquia>();
            if (jerarquias != null && jerarquias.Count > 0)
            {
                foreach (var item in jerarquias)
                {
                    jerarquiashijas.AddRange(ObtenerJerarquiasHijas(item.ComponenteInferiorId));
                }

                jerarquias.AddRange(jerarquiashijas);
            }

            return jerarquias;
        }

        public List<Jerarquia> ObtenerJerarquiasPadres(long idJerarquia)
        {
            var jerarquias = new List<Jerarquia>(_context.Jerarquia.Where(a => a.ComponenteInferiorId == idJerarquia));
            var jerarquiasPadres = new List<Jerarquia>();
            if (jerarquias != null && jerarquias.Count > 0)
            {
                foreach (var item in jerarquias)
                {
                    jerarquiasPadres.AddRange(ObtenerJerarquiasPadres(item.ComponenteSuperiorId));
                }

                jerarquias.AddRange(jerarquiasPadres);
            }

            return jerarquias;
        }

        public Jerarquia ObtenerJerarquiaPadre(long idJerarquia)
        {
            return _context.Jerarquia.FirstOrDefault(a => a.ComponenteSuperiorId == idJerarquia);
        }

        public Jerarquia ObtenerJerarquiaHijo(long idJerarquia)
        {
            return _context.Jerarquia.FirstOrDefault(a => a.ComponenteInferiorId == idJerarquia);
        }

        public bool QuitarObjetoColeccion(long usuarioId, long objetoId, long componenteId, long coleccionId)
        {
            var coleccionComponente = _context.ColeccionComponente.Where(c => c.ColeccionId == coleccionId && c.ComponenteId == componenteId && c.ObjetoId == objetoId);
            if (coleccionComponente == null)
                return false;

            foreach (var item in coleccionComponente)
            {
                item.FechaBaja = DateTime.Now;
                item.UsuarioBaja = usuarioId;
                _context.Entry(item).State = EntityState.Modified;
            }

            _context.SaveChanges();

            return true;
        }

        public bool AgregarObjetoColeccion(long usuarioId, long objetoId, long componenteId, long coleccionId)
        {
            var coleccionComponente = new ColeccionComponente
            {
                ColeccionId = coleccionId,
                ComponenteId = componenteId,
                ObjetoId = objetoId,
                UsuarioAlta = usuarioId,
                FechaAlta = DateTime.Now
            };

            _context.Entry(coleccionComponente).State = EntityState.Added;
            _context.SaveChanges();

            return true;
        }

        public ColeccionComponente GetColeccionComponenteByObjetoId(long objetoId)
        {
            return _context.ColeccionComponente.FirstOrDefault(c => c.ObjetoId == objetoId);
        }

        public bool AgregarColeccionComponente(ColeccionComponente coleccionComponente)
        {
            _context.Entry(coleccionComponente).State = EntityState.Added;
            _context.SaveChanges();

            return true;
        }

        public IEnumerable<Coleccion> GetColeccionesUsuarioByComponentesPrincipales(long idUsuario, long[] componentesPrincipales)
        {
            //Revisar como obtiene las colecciones, un query normal tarda mucho menos.
            var query = (from coleccion in _context.Coleccion
                         join colcomp in _context.ColeccionComponente on coleccion.ColeccionId equals colcomp.ColeccionId
                         join componente in _context.Componente on colcomp.ComponenteId equals componente.ComponenteId
                         join principal in componentesPrincipales on componente.ComponenteId equals principal
                         where coleccion.UsuarioAlta == idUsuario && coleccion.FechaBaja == null && colcomp.FechaBaja == null
                         select coleccion);

            return query.ToList().GroupBy(c => c.ColeccionId, (id, elementos) => elementos.FirstOrDefault());
        }

        public void GuardarColeccion(Coleccion coleccion)
        {
            _context.Coleccion.Add(coleccion);
            _context.SaveChanges();
        }


        public Coleccion GuardarColeccion(CargasTecnicas cargaTecnica)
        {
            Coleccion coleccion = new Coleccion() { Componentes = new List<ColeccionComponente>() };
            coleccion.FechaAlta = cargaTecnica.Fecha_Alta.Value;
            coleccion.FechaModificacion = coleccion.FechaAlta;
            coleccion.UsuarioAlta = cargaTecnica.Usuario_Alta;
            coleccion.UsuarioModificacion = coleccion.UsuarioAlta;
            coleccion.Nombre = string.Format("Carga por {0}-{1}", _context.AnalisisTecnicos.Find(cargaTecnica.Id_Analisis).Nombre, coleccion.FechaAlta.ToString("dd/MM/yyyy HH:mm"));

            _context.Coleccion.Add(coleccion);

            _context.ColeccionComponente.AddRange(coleccion.Componentes);

            _context.SaveChanges();

            return coleccion;
        }
    }
}
