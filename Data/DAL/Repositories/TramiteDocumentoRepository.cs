using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    public class TramiteDocumentoRepository : ITramiteDocumentoRepository
    {
        private readonly GeoSITMContext _context;

        public TramiteDocumentoRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<TramiteDocumento> GetTramitesDocumentos(long pIdTramiteDocumento)
        {
            return _context.Tramite_Documento.Where(n => n.Fecha_Baja == null && n.Id_Tramite_Documento == pIdTramiteDocumento).ToList();
        }

        public TramiteDocumento GetTramiteDocumentoById(long pIdTramiteDocumento)
        {
            return _context.Tramite_Documento.FirstOrDefault(n => n.Fecha_Baja == null && n.Id_Tramite_Documento == pIdTramiteDocumento);
        }

        public IEnumerable<TramiteDocumento> GetTramiteDocumentoByTramite(long pIdTramite)
        {
            var objetos = _context.Tramite_Documento.Where(n => n.Fecha_Baja == null && n.Id_Tramite == pIdTramite).ToList();

            foreach (var objeto in objetos)
            {
                _context.Entry(objeto).Reference(l => l.Documento).Load();

                if (objeto.Documento != null)
                {
                    _context.Entry(objeto.Documento).Reference(r => r.Tipo).Load();
                }
            }

            return objetos;
        }

        public void InsertTramiteDocumento(TramiteDocumento mTramiteDocumento)
        {
            mTramiteDocumento.Id_Usu_Modif = mTramiteDocumento.Id_Usu_Alta;
            mTramiteDocumento.Fecha_Alta = DateTime.Now;
            mTramiteDocumento.Fecha_Modif = mTramiteDocumento.Fecha_Alta;
            _context.Tramite_Documento.Add(mTramiteDocumento);
        }

        public void DeleteTramiteDocumento(TramiteDocumento mTramiteDocumento)
        {
            mTramiteDocumento.Id_Usu_Modif = mTramiteDocumento.Id_Usu_Baja;
            mTramiteDocumento.Fecha_Baja = DateTime.Now;
            mTramiteDocumento.Fecha_Modif = mTramiteDocumento.Fecha_Baja;
            _context.Tramite_Documento.Attach(mTramiteDocumento);
            _context.Entry(mTramiteDocumento).State = EntityState.Modified;
        }
    }
}
