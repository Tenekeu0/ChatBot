using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace VirtualAssistant.Controllers
{
    public class WikiService
    {
        private readonly HttpClient _httpClient;
        private readonly List<string> _wikiBaseUrls;

        public WikiService(List<string> wikiBaseUrls)
        {
            _httpClient = new HttpClient();
            _wikiBaseUrls = wikiBaseUrls;
        }

        public async Task<string> SearchWikiAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                Console.WriteLine("Query is empty or whitespace.");
                return null;
            }

            var tasks = _wikiBaseUrls.Select(async baseUrl =>
            {
                try
                {
                    var requestUrl = $"{baseUrl}?action=query&list=search&srsearch={System.Net.WebUtility.UrlEncode(query)}&format=json";
                    Console.WriteLine($"Request URL: {requestUrl}");  // Logging de l'URL
                    var response = await _httpClient.GetStringAsync(requestUrl);
                    var snippet = ExtractSnippet(response);
                    return string.IsNullOrEmpty(snippet) ? null : snippet;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error querying wiki: {ex.Message}");
                    return null;
                }
            });

            var responses = await Task.WhenAll(tasks);

            return responses.FirstOrDefault(response => !string.IsNullOrEmpty(response));
        }

        private string ExtractSnippet(string response)
        {
            var json = JObject.Parse(response);
            var query = json["query"];
            if (query != null)
            {
                var search = query["search"];
                if (search != null && search.Any())
                {
                    return search.FirstOrDefault()?["snippet"]?.ToString();
                }
            }
            return null;
        }
    }
}
