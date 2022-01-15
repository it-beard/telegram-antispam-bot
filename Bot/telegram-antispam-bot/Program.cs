using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramAntispam.Bot.Interfaces;
using TelegramAntispam.Bot.Services;

var botClient = new TelegramBotClient(Environment.GetEnvironmentVariable("TELEGRAM_ANTISPAM_BOT_KEY"));
using var cts = new CancellationTokenSource();

IDeleteMessageService deleteService = new DeleteMessageService();
IHandleMessageService handleService = new HandleMessageService(deleteService);

botClient.StartReceiving(
    HandleUpdateAsync,
    handleService.HandleErrorAsync,
    new ReceiverOptions(),
    cts.Token);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(
    ITelegramBotClient botClient,
    Update update,
    CancellationToken cancellationToken)
{
    handleService.HandleUpdateAsync(botClient, update, update.Type, cancellationToken);
}