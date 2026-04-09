using Investissement_WebClient.Core.InterfacesServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Investissement_WebClient.Data.Services
{
    public class TrTransactionsService : ITrTransactionsService
    {
        public async Task<string> GetSms()
        {
            using var client = new HttpClient();

            var content = new StringContent("application/json");
            var response = await client.PostAsync("http://89.168.42.226:5000/auth/request-sms", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(responseBody);
                var root = doc.RootElement;

                if (root.TryGetProperty("status", out var status))
                {
                    return status.GetString();
                }
            }

            return "erreur";
        }
    }
}
