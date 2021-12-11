namespace TelegramAntispam.Bot;

public class Settings
{
    public static string NoCommentWord = "#advert";
    
    //Add your telegram nickname here
    public static readonly List<string> WhiteList = new()
    {
        "iamitbeard",
        "SergShadow",
        "lparziva1l",
        "andreyolegovichru",
        "pilotInspektor",
        "igotodev",
        "Victor_BD"
    };

    public static readonly string InfoMessage =
        "Если ты хочешь отправлять ссылки в комментариях канала АйТиБорода, " +
        "то заходи в репозиторий github.com/it-beard/telegram-antispam-bot и вноси себя в WhiteList (подробнее читай в файле README.md репозитория)";
}
