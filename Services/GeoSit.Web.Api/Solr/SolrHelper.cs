using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GeoSit.Web.Api.Solr
{
    internal enum Entities
    {
        persona,
        parcela,
        prescripcion,
        parcelamunicipal,
        parcelaproyecto,
        unidadtributaria,
        parcelasaneable,
        mensura,
        manzana,
        calle,
        documento,
        tramite,
        parcelahistorica,
        unidadtributariahistorica,
        all
    }
    public class SolrUpdater
    {
        internal static SolrUpdater Instance { get { return __instance ?? (__instance = new SolrUpdater()); } }

        internal void Start()
        {
            stopFlag = false;
            Task.Run(process);
            return;
        }
        internal void Stop()
        {
            stopFlag = true;
            mtxProcessing.WaitOne();
            mtxProcessing.ReleaseMutex();
        }
        internal void Enqueue(Entities entity)
        {
            __queueUpdate.Enqueue(entity);
            if (mtxProcessing.WaitOne(10))
            {
                mtxProcessing.ReleaseMutex();
                Start();
            }
        }

        private readonly Queue<Entities> __queueUpdate;
        private static SolrUpdater __instance;
        private bool stopFlag;
        private Mutex mtxProcessing;
        private SolrUpdater()
        {
            __queueUpdate = new Queue<Entities>();
            mtxProcessing = new Mutex();
            stopFlag = true;
        }

        private Task process()
        {
            try
            {
                mtxProcessing.WaitOne();
                while (!stopFlag)
                {
                    if (!__queueUpdate.Any())
                    {
                        updateSuggest("Suggester");
                        stopFlag = !__queueUpdate.Any();
                        continue;
                    }
                    updateEntity(__queueUpdate.Dequeue());
                }
            }
            finally
            {
                mtxProcessing.ReleaseMutex();
            }
            return Task.FromResult(0);
        }

        private void updateEntity(Entities entity)
        {
            /* 
             * Si se está ejecutando una reindexacion, es inutil que mande a reindexar
             * ya que no se encola. 
             * Esto puede darse porque alguien mandó una reindexacion por fuera del 
             * sistema. Por ejemplo un full-import desde la consola de administracion
             * web de Solr 
             */
            if (isBusy()) return;

            doUpdate(entity);

            /*
             * Espero a que termine de reindexar para seguir procesando y no correr el 
             * riesgo de que la proxima actualización salga sin procesar
             */
            waitWhileBusy();
        }

        private void updateSuggest(string suggester)
        {
            var suggestParams = new List<KeyValuePair<string, string>>(new[] {
                new KeyValuePair<string, string>("suggest.dictionary", suggester),
                new KeyValuePair<string, string>("suggest.build", "true")
            });
            send("suggest", suggestParams);
        }

        private void doUpdate(Entities entity)
        {
            var updateParams = new List<KeyValuePair<string, string>>(new[]{
                new KeyValuePair<string, string>("command", "delta-import"),
                new KeyValuePair<string, string>("clean", "false"),
                new KeyValuePair<string, string>("optimize", "false"),
                new KeyValuePair<string, string>("softCommit", "true")
            });
            if(entity != Entities.all)
            {
                updateParams.Add(new KeyValuePair<string, string>("entity", entity.ToString()));
            }
            send("dataimport", updateParams);
        }

        private void waitWhileBusy()
        {
            while (isBusy())
            {
                Thread.Sleep(100);
            }
        }

        private bool isBusy()
        {
            var statusParams = new List<KeyValuePair<string, string>>(new[]{
                new KeyValuePair<string, string>("command", "status"),
                new KeyValuePair<string, string>("wt", "json")
            });

            string json = send("dataimport", statusParams);
            return JObject.Parse(json)["status"].ToString().ToLower() == "busy";
        }

        private string send(string command, List<KeyValuePair<string, string>> queryParams)
        {
            using (HttpClient cliente = new HttpClient(new HttpClientHandler { Credentials = System.Net.CredentialCache.DefaultNetworkCredentials }))
            {
                cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["solrUrl"]);
                cliente.Timeout = TimeSpan.FromMinutes(10);

                var resp = cliente.GetAsync(string.Format("{0}?{1}", command, string.Join("&", queryParams.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value))))).Result;
                resp.EnsureSuccessStatusCode();

                return resp.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
