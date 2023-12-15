using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Actions.Abstract;

namespace GeoSit.Data.DAL.Tramites.Actions
{
    class GenerarCertificadoCatastral : GenerarInformeEntrada
    {
        private const long ID_TIPO_INFORME = 12;
        public GenerarCertificadoCatastral(METramite tramite, GeoSITMContext contexto)
            : base(tramite, contexto, ID_TIPO_INFORME) { }

        protected override void AddInforme(METramiteEntrada entrada, Documento documento)
        {
            //se usa para asociar el documento a los objetos correspondientes
        }
        protected override void ExecuteEntrada(METramiteEntrada entrada)
        {
            /*
             * Acá iría la logica para generar el registro en INMCertificadoCatastral. Por ejemplo
                
                var certificadoCatastral = new INMCertificadoCatastral()
                {
                    Numero = numero,
                    FechaEmision = Convert.ToDateTime(fechaemision),
                    Motivo = motivo,
                    Vigencia = Convert.ToInt32(vigencia),
                    Descripcion = descripcion,
                    Observaciones = observaciones,
                    FechaAlta = DateTime.Now,
                    FechaModif = DateTime.Now,
                    UsuarioAltaId = idUsuario,
                    UsuarioModifId = idUsuario,

                    MensuraId = Convert.ToInt64(planoMensura),
                    MensuraVepId = string.IsNullOrEmpty(planoVep) ? null : (long?)Convert.ToInt64(planoVep),
                    SolicitanteId = Convert.ToInt64(solicitante),
                    UnidadTributariaId = Convert.ToInt64(partida)
                };
             */
        }
        protected override byte[] GetInforme(METramiteEntrada entrada)
        {
            using (var req = GetApiReportesClient())
            using (var resp = req.GetAsync($"api/InformeCertificadoCatastral/GetInforme?id={0}&usuario={_nombreCompletoUsuario}&tramite={Tramite.IdTramite}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsByteArrayAsync().Result;
            }
        }
        protected override string GetNombreArchivo(METramiteEntrada entrada)
        {
            return $"CertificadoCatastral_{Tramite.IdTramite}.pdf";
        }
    }
}