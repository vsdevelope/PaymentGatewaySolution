using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.Api.FunctionalTests.Helpers
{
    public static class ExtensionMethods
    {
        
        public static string[] AsStrings(this Table table, string column)
        {
            return table.Rows.Select(r => r[column]).ToArray();
        }

        public static string ToJson<T>(this T source) =>
            JsonConvert.SerializeObject(source);

        public static T FromJson<T>(this string json) =>
            JsonConvert.DeserializeObject<T>(json);

      
        public static async Task<Tuple<string, HttpStatusCode>> Get(
            this HttpClient client,
            string url,
            string merchantKey
            )
        {
            HttpResponseMessage response = null;
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
            {
                requestMessage.Headers.Add("merchantKey", merchantKey);
                response = await client.SendAsync(requestMessage);
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var responseCode = response.StatusCode;
            return new Tuple<string, HttpStatusCode>(responseString, responseCode);
        }

        public static async Task<Tuple<string, HttpStatusCode>> Post(
            this HttpClient client,
            string url,
            string payload,
            string merchantKey)
        {
            
            HttpResponseMessage response = null;
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, url))
            {
                requestMessage.Headers.Add("merchantKey", merchantKey);
                
                requestMessage.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                response = await client.SendAsync(requestMessage);
            }
            var responseString = await response.Content.ReadAsStringAsync();
            var responseCode = response.StatusCode;
            return new Tuple<string, HttpStatusCode>(responseString, responseCode);
        }
    }
}