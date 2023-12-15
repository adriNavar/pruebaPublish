using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Common
{
    public class Enumerators
    {
        public enum EnumTipoMovimiento
        {
            None = 0,
            Crear = 1,
            Editar = 2,
            Anular = 3,
            Derivar = 4,
            Recibir = 5,
            Anular_derivacion = 6,
            Finalizar = 7,
            Despachar = 8,
            Rechazar = 9,
            Reingresar = 10,
            Presentar = 11,
            Confirmar = 12,
            Anular_Carga_Libro = 13,
            Archivar = 14,
            Desarchivar = 15,
            Recibir_Presentado = 16,
            Entregar = 17,
            Procesar = 18,
            Reasignar = 19
        }
        public enum EnumEstadoTramite
        {
            None = 0,
            Provisorio = 1,
            Presentado = 2,
            Iniciado = 3,
            En_proceso = 4,
            Derivado = 5,
            Anulado = 6,
            Despachado = 7,
            Rechazado = 8,
            Finalizado = 9,
            Archivado = 10,
            Procesado = 11,
            Entregado = 12
        }
        public enum EnumEvento
        {
            DerivarTramite = 137,
            RecibirTramite = 138,
            ConfirmarTramite = 139,
            AnularCargaLibroTramite = 140,
            RechazarTramite = 141,
            FinalizarTramite = 142,
            AnularTramite = 143,
            DespacharTramite = 144,
            ArchivarTramite = 145,
            RecibirPresentadoTramite = 146,
            AnularDerivaciónTramite = 147,
            DesarchivarTramite = 148,
            ReingresarTramite = 149,
            EditarTramite = 150,
            CrearTramite = 151,
            ImprimirCaratula = 152,
            ImprimirInformeDetallado = 153,
            ConsultarTramite = 164,
            EntregarTramite = 165,
            ProcesarTramite = 192,
            PresentarTramite = 206,
            ImprimirInformeAdjudicacion = 207,
            NotificarTramite = 208,
            ReasignarTramite = 223
        }
        public enum EnumTipoTramite
        {
            Oficio_Judicial = 31,
            Mensura = 1,
            Titulo = 2,
            Certificados = 3,
            Expediente = 4,
            Otros = 5
        }
    }
}
