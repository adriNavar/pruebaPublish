using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Enums;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Enums;
using GeoSit.Data.DAL.Tramites.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace GeoSit.Data.DAL.Tramites.Actions.Abstract
{
    abstract class Accion : IAction
    {
        protected GeoSITMContext Contexto { get; private set; }
        protected METramite Tramite { get; private set; }
        public List<string> Errores { get; protected set; }
        public ResultadoValidacion Resultado { get; protected set; }
        public Ambito Ambito { get; protected set; }

        protected Accion(METramite tramite, GeoSITMContext contexto)
        {
            Ambito = Ambito.Tramite;
            Contexto = contexto;
            Tramite = tramite;
        }
        protected virtual IQueryable<METramiteEntrada> GetEntradas(int idEntrada)
        {
            return from mte in Contexto.TramitesEntradas
                   join moe in Contexto.ObjetosEntrada on mte.IdObjetoEntrada equals moe.IdObjetoEntrada
                   where moe.IdEntrada == idEntrada && mte.IdTramite == Tramite.IdTramite
                   select mte;
        }
        abstract public bool Execute();
    }
}
