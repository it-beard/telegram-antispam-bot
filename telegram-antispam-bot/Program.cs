using TelegramAntispam.Bot;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var botClient = new TelegramBotClient(Environment.GetEnvironmentVariable("TelegramAntispamBotKey"));
using var cts = new CancellationTokenSource();

botClient.StartReceiving(
    HandleUpdateAsync,
    HandleErrorAsync,
    new ReceiverOptions(),
    cts.Token);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    try
    {

        // Only process Messages with URLs from users not in WhiteList
        if (update.Type != UpdateType.Message || 
            update.Message == null ||
            !update.Message.Entities.Select(e => e.Type).Contains(MessageEntityType.Url) ||
            update.Message.From != null && Settings.WhiteList
                .Any(w => update.Message.From.Username.ToLower().Contains(w.ToLower())))
        {
            return;
        }

        var chatId = update.Message.Chat.Id;
        var messageText = update.Message.Text;
        var messageId = update.Message.MessageId;
        Console.WriteLine($"Message for delete: '{messageText}' (id: {messageId})");

        await botClient.DeleteMessageAsync(chatId, messageId, cancellationToken);

        if (update.Message.From != null)
        {
            await botClient.SendTextMessageAsync(update.Message.From.Id, Settings.InfoMessage);
        }
    }
    catch (Exception ex)
    {
        //todo: catch later. For now just skipp and continue receiving messages
    }
}

async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    // todo: exception here stopped receiving messages...
}