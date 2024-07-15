using VirtualAssistant.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder;
using VirtualAssistant.ChatBots;
using Microsoft.Bot.Builder.AI.Luis;
using VirtualAssistant.Notifications;
using Autofac.Core;
using VirtualAssistant.Controllers;

namespace VirtualAssistant
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Connexion à la base de données
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Intégration du chatbot
            builder.Services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
            builder.Services.AddTransient<IBot, EchoBot>();
            builder.Services.AddHttpClient<BotController>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

        // Ajout de l'IA du langage naturel (LUIS)
        var luisApplication = new LuisApplication(
                builder.Configuration["Luis:ApplicationId"],
                builder.Configuration["Luis:EndpointKey"],
                builder.Configuration["Luis:Endpoint"]
            );

            var recognizerOptions = new LuisRecognizerOptionsV3(luisApplication)
            {
                PredictionOptions = new Microsoft.Bot.Builder.AI.LuisV3.LuisPredictionOptions
                {
                    IncludeAllIntents = true,
                    IncludeInstanceData = true
                }
            };

            builder.Services.AddSingleton(new LuisRecognizer(recognizerOptions));
            // Configure WitAiService with the access token from configuration
            builder.Services.AddSingleton(new WitAiService(builder.Configuration["WitAi:AccessToken"]));
            // Configure WikiService with the access token from configuration

            var wikiUrls = builder.Configuration.GetSection("Wiki:BaseUrls").Get<List<string>>();
            var pat = builder.Configuration["Wiki:Pat"];
            builder.Services.AddSingleton(new WikiService(wikiUrls));

            // Notifications
            builder.Services.AddSignalR();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "VirtualAssistant v1");
                });
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseCors("AllowAllOrigins");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/notificationHub");
            });

            app.Run();
        }
    }
}
