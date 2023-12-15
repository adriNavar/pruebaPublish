using System.Data;
using System.Linq;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.BusinessEntities.Documentos;
using System.Collections.Generic;
using System;
using GeoSit.Data.BusinessEntities.GlobalResources;

namespace GeoSit.Data.DAL.Repositories.Temporal
{
    public class MensuraTemporalRepository : IMensuraTemporalRepository
    {
        private readonly GeoSITMContext _context;

        public MensuraTemporalRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public MensuraTemporal GetMensura(int idMensura, int tramite)
        {
            return _context.MensurasTemporal.Find(idMensura, tramite);
        }

        public List<MensuraTemporal> GetEntradasByIdTramite(int idTramite)
        {
            int tipoEntradaMR = Convert.ToInt32(Entradas.MensuraRegistrada);

            var query = from entradaTramite in _context.TramitesEntradas
                        join objetoEntrada in _context.ObjetosEntrada on entradaTramite.IdObjetoEntrada equals objetoEntrada.IdObjetoEntrada
                        join par in _context.MensurasTemporal on new { id = entradaTramite.IdObjeto.Value, entradaTramite.IdTramite } equals new { id = par.IdMensura, par.IdTramite }
                        where objetoEntrada.IdEntrada == tipoEntradaMR && idTramite == entradaTramite.IdTramite
                        select par;

            return query.ToList();
        }

        //MENSURA Y TIPOMENSURA PERTENECEN AL ESQUEMA GEOSITM 
        public TipoMensura GetTipoMensura(long idMensura)
        {
            var query = (from men in _context.Mensura
                         join tm in _context.TipoMensura on men.IdTipoMensura equals tm.IdTipoMensura
                         where idMensura == men.IdMensura
                         select tm).FirstOrDefault();

            return query;
        }

        public Documento GetDocumentoMensura(long idMensura)
        {
            var query = (from md in _context.MensuraDocumento
                         join d in _context.Documento on md.IdDocumento equals d.id_documento
                         where idMensura == md.IdMensura
                         select d).FirstOrDefault();

            return query;
        }
    }
}
