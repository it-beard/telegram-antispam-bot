using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramAntispam.Bot.Interfaces;

namespace TelegramAntispam.Bot.Services;

public class HandleMessageService : IHandleMessageService
{
    private readonly IDeleteMessageService deleteMessageService;
    public HandleMessageService(IDeleteMessageService deleteMessageService)
    {
        this.deleteMessageService = deleteMessageService;
    }
    
    public async Task HandleUpdateAsync(
        ITelegramBotClient botClient, 
        Update update, 
        UpdateType type,
        CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"Message log: {JsonConvert.SerializeObject(update)}");
            if (update.HasEmptyMessage())
            {
                return;
            }
        
            switch (type)
            {
                // Disable comments if new post contains no-comment word
                case UpdateType.Message when update.Message.From.IsChannel() &&
                                             update.Message.Text.Contains(Settings.NoCommentWord):
                // Delete new comment with link if user not in white-list
                case UpdateType.Message when update.Message.ContainsUrls() && 
                                             !update.Message.From.IsChannel() &&
                                             !update.Message.From.InWhitelist():
                    await deleteMessageService.DeleteMessageAsync(botClient, update.Message, cancellationToken);
                    break;
                // Disable comments if edited post contains no-comment word
                case UpdateType.EditedMessage when update.EditedMessage.From.IsChannel() &&
                                                   update.EditedMessage.Text.Contains(Settings.NoCommentWord):
                // Delete edited comment with link if user not in white-list
                case UpdateType.EditedMessage when update.EditedMessage.ContainsUrls() && 
                                                   !update.EditedMessage.From.IsChannel() &&
                                                   !update.EditedMessage.From.InWhitelist():
                    await deleteMessageService.DeleteMessageAsync(botClient, update.EditedMessage, cancellationToken);
                    break;
                default:
                    return;
            }
        }
        catch (Exception ex)
        {
            //todo: catch later. For now just skipp and continue receiving messages
        }
    }

    public async Task HandleErrorAsync(
        ITelegramBotClient botClient, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        // todo: exception here stopped receiving messages...
    }
}