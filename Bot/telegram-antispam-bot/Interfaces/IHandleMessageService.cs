using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramAntispam.Bot.Interfaces;

public interface IHandleMessageService
{
    Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        UpdateType type,
        CancellationToken cancellationToken);

    Task HandleErrorAsync(
        ITelegramBotClient botClient,
        Exception exception,
        CancellationToken cancellationToken);
}