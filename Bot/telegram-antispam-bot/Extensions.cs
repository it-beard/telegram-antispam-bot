using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramAntispam.Bot;

public static class Extensions
{
    public static bool ContainsUrls(this Message message)
    {
        return message is {Entities: { }} 
               && message.Entities.Select(e => e.Type).Contains(MessageEntityType.Url);
    }
    
    public static bool InWhitelist(this User user)
    {
        return user is {Username: { }}  && 
               Settings.WhiteList.Any(w => user.Username.ToLower().Contains(w.ToLower()));
    }
    
    public static bool IsChannel(this User user)
    {
        return user is {FirstName: "Telegram"};
    }
}