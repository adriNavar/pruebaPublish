using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace GeoSit.Client.Web.Solr
{
    internal class SolrServer
    {
        internal static string EscapeSpecialChars(string text)
        {
            return new[] { "+", "-", "&&", "||", "!", "(", ")", "\"", "~", "*", "?", ":" }
                            .Aggregate(text, (escaped, escape) => escaped.Replace(escape, $"\\{escape}"));            
        }
        private readonly string url;
        private readonly List<SolrParam> parameters;

        internal bool UseDefaultBaseParams { get; set; }
        internal bool UseDefaultFacetParams { get; set; }
        internal bool UseDefaultGroupParams { get; set; }
        internal bool UseDefaultSuggestParams { get; set; }
        internal SolrServer()
        {
            this.url = ConfigurationManager.AppSettings["solrUrl"].ToString();
            this.parameters = new List<SolrParam>();
        }
        internal void AddParam(SolrParam param)
        {
            this.parameters.Add(new SolrParam(param.Key, param.Value));
        }
        internal void Clear()
        {
            this.parameters.Clear();
        }
        internal string Suggest(string query)
        {
            return get("suggest", query);
        }
        internal string Search(string query)
        {
            Regex regexTerms = new Regex(@"^\({!terms", RegexOptions.IgnoreCase);
            Regex regexNormal = new Regex(@"^[\*\wa-z0-9]{1}.*$", RegexOptions.IgnoreCase);
            if (!regexTerms.IsMatch(query) && !regexNormal.IsMatch(query))
                query = string.Empty;

            return get("select", query);
        }
        internal void Update(string data)
        {
            this.AddParam(new SolrParam("versions", "true"));
            this.AddParam(new SolrParam("softCommit", "true"));
            this.post("update", new StringContent(data, Encoding.UTF8, "application/json"));
        }
        internal bool Indexing()
        {
            //var statusParams = new List<KeyValuePair<string, string>>(new[]{
            //    new KeyValuePair<string, string>("command", "status"),
            //    new KeyValuePair<string, string>("wt", "json")
            //});

            //string json = send("dataimport", statusParams);
            //return JObject.Parse(json)["status"].ToString().ToLower() == "busy";
            var parametros = new SolrParam[]
            {
                new SolrParam("command", "status"),
                new SolrParam("wt", "json")
            };
            string resp = post("dataimport", new StringContent(this.formatParams(parametros), Encoding.UTF8, "application/x-www-form-urlencoded"));
            return JObject.Parse(resp)["status"].ToString().ToLower() == "busy";
        }
        private string get(string command, string query)
        {
            if (this.getParams().Any())
            {
                this.AddParam(new SolrParam("q", query));
            }
            using (var cliente = this.getHttpCliente())
            using (var resp = cliente.PostAsync(command, new StringContent(this.formatParams(this.parameters), Encoding.UTF8, "application/x-www-form-urlencoded")).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsStringAsync().Result;
            }
        }
        private string post(string command, HttpContent content)
        {
            using (var cliente = this.getHttpCliente())
            {
                var resp = cliente.PostAsync($"{command}?{this.formatParams()}", content).Result;
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsStringAsync().Result;
            }
        }
        private string formatParams(IEnumerable<SolrParam> parameters = null)
        {
            return string.Join("&", (parameters ?? this.getParams()).Select(kv => $"{kv.Key}={kv.Value}"));
        }
        private IEnumerable<SolrParam> getParams()
        {
            var addParam = new Action<string>(p => this.AddParam(new SolrParam(p.Split('=')[0], p.Split('=')[1])));
            if (this.UseDefaultBaseParams)
            {
                Array.ForEach(ConfigurationManager.AppSettings["solrBaseParams"].ToString().Split('&'), addParam);
            }
            if (this.UseDefaultFacetParams)
            {
                Array.ForEach(ConfigurationManager.AppSettings["solrFacetParams"].ToString().Split('&'), addParam);
            }
            if (this.UseDefaultGroupParams)
            {
                Array.ForEach(ConfigurationManager.AppSettings["solrGroupParams"].ToString().Split('&'), addParam);
            }
            if (this.UseDefaultSuggestParams)
            {
                Array.ForEach(ConfigurationManager.AppSettings["solrSuggestParams"].ToString().Split('&'), addParam);
            }
            return this.parameters;
        }

        internal string Terms(IEnumerable<KeyValuePair<int, string[]>> data)
        {
            var filtros = data.GroupBy(e => e.Key);

            List<string> lista = new List<string>();
            foreach (var grupo in filtros)
            {
                List<string> andList = new List<string>();
                foreach (var filtro in grupo)
                {
                    andList.Add($"{{!terms f={filtro.Value[0]}}}{filtro.Value[1]}");
                }
                lista.Add($"({string.Join(" AND ", andList)})");
            }
            return this.Search(string.Join(" OR ", lista));
        }

        private HttpClient getHttpCliente(string otherurl = null)
        {
            return new HttpClient { BaseAddress = new Uri(otherurl ?? this.url) };
        }
    }

    internal class SolrParam
    {
        internal string Key { get; private set; }
        internal string Value { get; private set; }

        internal SolrParam(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}