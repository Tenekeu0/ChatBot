using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;

namespace VirtualAssistant.ChatBots
{
    public class EchoBot : ActivityHandler
    {
        private readonly LuisRecognizer _luisRecognizer;
        private readonly ILogger<EchoBot> _logger;

        public EchoBot(LuisRecognizer luisRecognizer, ILogger<EchoBot> logger)
        {
            _luisRecognizer = luisRecognizer;
            _logger = logger;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var recognizerResult = await _luisRecognizer.RecognizeAsync(turnContext, cancellationToken);
            var topIntent = recognizerResult.GetTopScoringIntent();

            if (topIntent.intent == "None" || topIntent.score < 0.7)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("I'm not sure what you mean. Can you please rephrase?"), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"LUIS detected the intent: {topIntent.intent} with score: {topIntent.score}"), cancellationToken);
            }
        }
        // Méthode pour envoyer un message de bienvenue aux nouveaux utilisateurs
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Bienvenue! Je suis l'assistant virtuel. Comment puis-je vous aider aujourd'hui?";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
