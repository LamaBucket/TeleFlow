using demo;
using demo.Services;
using LisBot.Common.Telegram.Factories;
using LisBot.Common.Telegram.Models;
using LisBot.Common.Telegram.Services;
using Telegram.Bot;
using Telegram.Bot.Extensions.Handlers.Services;
using Telegram.Bot.Extensions.Handlers.Services.Markup;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

string BOT_TOKEN = System.IO.File.ReadAllText("bot-token.txt");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<IMessageServiceFactory<IMessageServiceWithEdit<Message>, Message>, UniversalMessageServiceFactory>();
builder.Services.AddSingleton<IMessageServiceFactory<string>, UniversalMessageServiceFactory>();
builder.Services.AddSingleton<IMessageServiceFactory<ImageMessageServiceMessage>, UniversalMessageServiceFactory>();



builder.Services.AddSingleton<IReplyMarkupManagerFactory, ReplyMarkupManagerFactory>();
builder.Services.AddSingleton<InlineMarkupManagerFactory, DemoInlineMarkupManagerFactory>();

builder.Services.AddSingleton<IMediaDownloaderServiceFactory, MediaDownloaderServiceFactory>();

builder.Services.AddSingleton<IUpdateDistributorArgsBuilder<UpdateDistributorNextHandlerBuildArgs>, UpdateDistributorArgsBuilder>();

builder.Services.AddSingleton<UpdateDistributorFactory<UpdateDistributorNextHandlerBuildArgs>, DemoUpdateDistributorFactory>();

builder.Services.AddHttpClient("tgwebhook")
                .RemoveAllLoggers()
                .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(BOT_TOKEN, httpClient));

builder.Services.ConfigureTelegramBotMvc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
