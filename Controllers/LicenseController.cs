using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CliquedinSeguir.Controllers.Arka
{
    public class APIReturn
    {
        [JsonProperty("status")]
        public int Status { get; set; }
        [JsonProperty("expiration")]
        public string Expiration { get; set; }
        [JsonProperty("check")]
        public bool Check { get; set; }
    }

    public static class LicenseController
    {
        public static async Task<bool> License()
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://arkabot.com.br/api/checkcliquedin"));
                if (response.IsSuccessStatusCode)
                {
                    APIReturn ret = JsonConvert.DeserializeObject<APIReturn>(await response.Content.ReadAsStringAsync());
                    if (String.IsNullOrEmpty(ret.Expiration)) return true;
                    return ret.Check;
                }
                else
                {
                    return true;
                }
            } catch
            {
                return true;
            }
        }

        public static async Task Open()
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://arkabot.com.br/api/cliquedinadd?type=seguir"));
                return;
            }
            catch
            {
                return;
            }
        }

    }
}
