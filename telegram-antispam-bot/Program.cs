using TelegramAntispam.Bot;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var botClient = new TelegramBotClient(Environment.GetEnvironmentVariable("TELEGRAM_ANTISPAM_BOT_KEY"));
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

async Task HandleUpdateAsync(
    ITelegramBotClient botClient, 
    Update update, 
    CancellationToken cancellationToken)
{
    try
    {
        // do not delete messages from "channel" user type
        if (update.Message != null && update.Message.From.IsChannel())
        {
            // Comments are disabled if channel message contains NoCommentWord
            if (update.Message.Text.Contains(Settings.NoCommentWord))
            {
                await DeleteMessageAsync(botClient, update.Message, cancellationToken);
            }
            
            return;
        }
        
        switch (update.Type)
        {
            case UpdateType.Message when update.Message.ContainsUrls() && 
                                         !update.Message.From.InWhitelist():
                await DeleteMessageAsync(botClient, update.Message, cancellationToken);
                break;
            case UpdateType.EditedMessage when update.EditedMessage.ContainsUrls() && 
                                               !update.EditedMessage.From.InWhitelist():
                await DeleteMessageAsync(botClient, update.EditedMessage, cancellationToken);
                break;
        }
    }
    catch (Exception ex)
    {
        //todo: catch later. For now just skipp and continue receiving messages
    }
}

async Task HandleErrorAsync(
    ITelegramBotClient botClient, 
    Exception exception, 
    CancellationToken cancellationToken)
{
    // todo: exception here stopped receiving messages...
}

async Task DeleteMessageAsync(
    ITelegramBotClient botClient,
    Message message,
    CancellationToken cancellationToken)
{
    var chatId = message.Chat.Id;
    var messageText = message.Text;
    var messageId = message.MessageId;
    Console.WriteLine($"Message for delete: '{messageText}' (id: {messageId})");

    await botClient.DeleteMessageAsync(chatId, messageId, cancellationToken);

    if (message.From is {Username: { }})
    {
        await botClient.SendTextMessageAsync(message.From.Id, Settings.InfoMessage);
    }
}