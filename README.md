# [EN] Telegram Antispam Bot
Simple bot deletes all messages with URLs  
Also bot has Whitelist of telegram's nicknames that allowed to send URLs  
Whitelist is here: `Settings.cs`  

Based on .NET 6  
Using https://github.com/TelegramBots/Telegram.Bot as a .NET Client for Telegram Bot API.

# [RU] Антиспам telegram-бот
Это простой бот, который удаляет все сообщения, содержащие ссылки.  
Для того, чтобы бот работал, его необходимо добавить в администраторы чата в TG.  

У бота есть настраиваемый белый список пользователей, которым разрешено писать сообщения со ссылками.
Also bot have Whitelist of telegram's nicknames that allowed to send URLs  
Белый список находится в файле: `Settings.cs`.

Этот бот работает как спам-фильтр в telegram-канале [АйТиБорода](https://t.me/itbeard)  
Если вы хотите иметь возможность слать ссылки в комментариях к постам канала "АйТиБорода", то просто сделайте форк этого репозитория, добавьте свой никнейм в `Settings.cs` и создайте пуллреквест с пояснением, почему именно вас необходимо внести в белый список (почему вы не спамер).



