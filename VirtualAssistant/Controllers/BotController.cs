using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using System.Threading.Tasks;
using VirtualAssistant.Models;
using VirtualAssistant.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using HtmlAgilityPack;
using Markdig;
using Newtonsoft.Json;

namespace VirtualAssistant.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly WitAiService _witAiService;
        private readonly AppDbContext _dbContext;
        private readonly WikiService _wikiService;
        private readonly HttpClient _httpClient;

        public BotController(AppDbContext dbContext, WikiService wikiService, HttpClient httpClient)
        {
            _witAiService = new WitAiService("7ERXWFQ53GST4ROJ2JXWXYHYTEZB5O33");
            _dbContext = dbContext;
            _wikiService = wikiService;
            _httpClient = httpClient;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] MessageRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Text))
            {
                return BadRequest("Le message ne peut pas être vide.");
            }

            var topIntent = await _witAiService.GetTopIntentAsync(request.Text);

            string botResponse = await GetResponseFromDatabase(topIntent);

            if (string.IsNullOrEmpty(botResponse))
            {
                botResponse = await GetResponseFromAzureDevOpsWiki(request.Text);
            }

            if (string.IsNullOrEmpty(botResponse))
            {
                botResponse = "Désolé, je n'ai pas trouvé de réponse appropriée.";
            }

            var response = new MessageResponse
            {
                Text = botResponse
            };

            return Ok(response);
        }

        private async Task<string> GetResponseFromDatabase(string intent)
        {
            var response = await _dbContext.Responses
                .Where(r => r.Response == intent)
                .Select(r => r.ResponseText)
                .FirstOrDefaultAsync();

            return response;
        }
        public async Task<string> GetResponseFromAzureDevOpsWiki(string query)
        {
            // Configurer l'URL et l'authentification
            var pat = "fnyjckc3ddj5uumqgcmyotaxnwcxvoa65q7yffilz2zk445plbva";
            var organization = "2292644";
            var project = "2292644-TP2";
            var wiki = "2292644-TP2.wiki";
            var pageId = "1"; // L'ID de la page est visible dans l'URL que vous avez fournie
            var apiVersion = "6.0";

            var wikiUrl = $"https://dev.azure.com/{organization}/{project}/_apis/wiki/wikis/{wiki}/pages/{pageId}?includeContent=true&api-version={apiVersion}";

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($":{pat}")));

            var response = await _httpClient.GetAsync(wikiUrl);
            if (response.IsSuccessStatusCode)
            {
                var wikiResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response: {wikiResponse}");
                var wikiPage = JsonConvert.DeserializeObject<WikiPage>(wikiResponse);

                if (wikiPage != null && !string.IsNullOrEmpty(wikiPage.Content))
                {
                    var markdownPipeline = new MarkdownPipelineBuilder().Build();
                    var html = Markdown.ToHtml(wikiPage.Content, markdownPipeline);

                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(html);
                    var plainText = htmlDocument.DocumentNode.InnerText;

                    var relevantText = FindRelevantText(plainText, query);

                    if (!string.IsNullOrEmpty(relevantText))
                    { 
                        return relevantText;
                    }

                    var defaultResponse = GetDefaultResponse(plainText);
                    if (!string.IsNullOrEmpty(defaultResponse))
                    {
                        return defaultResponse;
                    }
                }
            }

            return "Désolé, je n'ai pas pu trouver d'information pertinente.";
        }

        private string FindRelevantText(string plainText, string query)
        {
            var lines = plainText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var keywords = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var relevantLines = lines
                .Select((line, index) => new { Line = line, Index = index, Score = keywords.Count(k => line.ToLower().Contains(k)) })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .Take(3)
                .ToList();

            if (relevantLines.Any())
            {
                var context = new List<string>();
                foreach (var item in relevantLines)
                {
                    int startIndex = Math.Max(0, item.Index - 1);
                    int endIndex = Math.Min(lines.Length - 1, item.Index + 1);

                    for (int i = startIndex; i <= endIndex; i++)
                    {
                        context.Add(lines[i]);
                    }
                }

                return string.Join("\n", context.Distinct());
            }

            return null;
        }

        private string GetDefaultResponse(string plainText)
        {
            var lines = plainText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var importantKeywords = new[] { "objectif", "étape", "remise", "projet", "pipeline" };

            var relevantLines = lines
                .Select((line, index) => new { Line = line, Index = index, Score = importantKeywords.Count(k => line.ToLower().Contains(k)) })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .Take(3)
                .Select(x => x.Line)
                .ToList();

            if (relevantLines.Any())
            {
                return "Voici quelques informations importantes du projet :\n" + string.Join("\n", relevantLines);
            }

            return "Je n'ai pas pu trouver d'informations spécifiques. Le document contient des détails sur un travail pratique de déploiement continu.";
        }
    }

    public class WikiPage
    {
        public string Path { get; set; }
        public int Order { get; set; }
        public bool IsParentPage { get; set; }
        public string GitItemPath { get; set; }
        public List<WikiPage> SubPages { get; set; }
        public string Url { get; set; }
        public string RemoteUrl { get; set; }
        public string Content { get; set; }
    }
}





/*
 token:  fnyjckc3ddj5uumqgcmyotaxnwcxvoa65q7yffilz2zk445plbva
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.AI.Luis;
using System.Threading.Tasks;
using VirtualAssistant.Models;
using VirtualAssistant.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace VirtualAssistant.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly LuisRecognizer _luisRecognizer;
        private readonly AppDbContext _dbContext;

        public BotController(LuisRecognizer luisRecognizer, AppDbContext dbContext)
        {
            _luisRecognizer = luisRecognizer;
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] MessageRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Text))
            {
                return BadRequest("Le message ne peut pas être vide.");
            }

            var luisResult = await _luisRecognizer.RecognizeAsync(request.Text);

            var topIntent = luisResult.GetTopScoringIntent();

            string botResponse = await GetResponseFromDatabase(topIntent.intent);

            var response = new MessageResponse
            {
                Text = botResponse
            };

            return Ok(response);
        }

        private async Task<string> GetResponseFromDatabase(string intent)
        {
            var response = await _dbContext.Responses
                .Where(r => r.Response == intent)
                .Select(r => r.ResponseText)
                .FirstOrDefaultAsync();

            return response ?? "Désolé, je n'ai pas trouvé de réponse pour cette intention.";
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using System.Threading.Tasks;

namespace VirtualAssistant.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly IBot _bot;

        public BotController(IBotFrameworkHttpAdapter adapter, IBot bot)
        {
            _adapter = adapter;
            _bot = bot;
        }

        [HttpPost]
        public async Task PostAsync()
        {
            await _adapter.ProcessAsync(Request, Response, _bot);
        }
    }
}

*/