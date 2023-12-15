using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using System;

namespace GeoSit.Data.DAL.Repositories
{
    public class ParcelaDocumentoRepository: IParcelaDocumentoRepository
    {
        private readonly GeoSITMContext _context;

        public ParcelaDocumentoRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public ParcelaDocumento GetParcelaDocumentoById(long idParcelaDocumento)
        {
            return _context.ParcelasDocumentos.FirstOrDefault(pd => pd.ParcelaDocumentoID == idParcelaDocumento);
        }

        public void InsertParcelaDocumento(ParcelaDocumento parcelaDocumento)
        {
            parcelaDocumento.UsuarioAltaID = parcelaDocumento.UsuarioModificacionID;
            parcelaDocumento.FechaModificacion = DateTime.Now;
            parcelaDocumento.FechaAlta = parcelaDocumento.FechaModificacion;
            _context.ParcelasDocumentos.Add(parcelaDocumento);
        }

        public void DeleteParcelaDocumento(ParcelaDocumento parcelaDocumento)
        {
            parcelaDocumento.UsuarioBajaID = parcelaDocumento.UsuarioModificacionID;
            parcelaDocumento.FechaModificacion = DateTime.Now;
            parcelaDocumento.FechaBaja = parcelaDocumento.FechaModificacion;
            _context.Entry(parcelaDocumento).State = EntityState.Modified;
        }

        public IEnumerable<AtributosDocumento> GetPlanoMensura(long idParcela)
        {
            int idTipo = Convert.ToInt32(_context.ParametrosGenerales.SingleOrDefault(p => p.Descripcion == "PLANO_DE_MENSURA_APROBADO").Valor);
            //var parcelasDocumentos = _context.ParcelasDocumentos.Include(p => p.Documento).Where(p=>p.FechaBaja == null && p.ParcelaID == idParcela);
            var query = from parcelaDocumento in _context.ParcelasDocumentos
                        join documento in _context.Documento on parcelaDocumento.DocumentoID equals documento.id_documento
                        where parcelaDocumento.ParcelaID == idParcela && documento.id_tipo_documento == idTipo && parcelaDocumento.FechaBaja == null
                        select documento;

            List<AtributosDocumento> atributosDocumento = new List<AtributosDocumento>();


            foreach ( var doc in query )
            {
                if (doc.atributos != null)
                { 
                    System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                    xml.LoadXml(doc.atributos);
                    AtributosDocumento aDoc = new AtributosDocumento();
                    aDoc.fecha_aprobacion = DateTime.ParseExact(xml.SelectSingleNode("/datos/fecha_aprobacion").InnerText, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentCulture);
                    aDoc.fecha_mensura = DateTime.ParseExact(xml.SelectSingleNode("/datos/fecha_mensura").InnerText, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentCulture);
                    aDoc.fecha_presentacion = DateTime.ParseExact(xml.SelectSingleNode("/datos/fecha_presentacion").InnerText, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentCulture);
                    aDoc.fecha_vigencia_actual = DateTime.ParseExact(xml.SelectSingleNode("/datos/fecha_vigencia_actual").InnerText, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentCulture);
                    aDoc.fecha_vigencia_original = DateTime.ParseExact(xml.SelectSingleNode("/datos/fecha_vigencia_original").InnerText, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentCulture);
                    aDoc.letra_plano = Convert.ToString(xml.SelectSingleNode("/datos/letra_plano").InnerText, System.Globalization.CultureInfo.CurrentCulture);
                    aDoc.mensuras_relacionadas = DateTime.ParseExact(xml.SelectSingleNode("/datos/mensuras_relacionadas").InnerText, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentCulture);
                    //aDoc.numero_plano = Convert.ToString(xml.SelectSingleNode("/datos/numero_plano").InnerText, System.Globalization.CultureInfo.CurrentCulture);
                    aDoc.numero_plano = Convert.ToInt64(xml.SelectSingleNode("/datos/numero_plano").InnerText, System.Globalization.CultureInfo.CurrentCulture);
                    atributosDocumento.Add(aDoc);
                }
            }
            return atributosDocumento;

        }
    }
}
