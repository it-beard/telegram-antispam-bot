namespace TelegramAntispam.Bot;

public class Settings
{
    //Add your telegram nickname here
    public static List<string> WhiteList = new()
    {
        "SergShadow"
    };

    public static string InfoMessage =
        "Если ты хочешь отправлять ссылки в комментариях канала АйТиБорода, " +
        "то заходи в репозиторий и вноси себя в WhiteList (файл Settings.cs)";
}