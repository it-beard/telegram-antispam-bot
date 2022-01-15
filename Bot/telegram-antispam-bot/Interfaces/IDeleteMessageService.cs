using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramAntispam.Bot.Interfaces;

public interface IDeleteMessageService
{
    Task DeleteMessageAsync(
        ITelegramBotClient botClient,
        Message message,
        CancellationToken cancellationToken);
}