using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace VirtualAssistant.Controllers
{
    public class WitAiService
    {
        private readonly string _accessToken;
        private readonly HttpClient _httpClient;

        public WitAiService(string accessToken)
        {
            _accessToken = accessToken;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
        }

        public async Task<string> GetTopIntentAsync(string message)
        {
            var response = await _httpClient.GetStringAsync($"https://api.wit.ai/message?v=20230307&q={System.Net.WebUtility.UrlEncode(message)}");
            var json = JObject.Parse(response);
            var intents = json["intents"];
            if (intents != null && intents.Any())
            {
                return intents.First()["name"].ToString();
            }
            return null;
        }
    }
}
