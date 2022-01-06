using Discord;

namespace Main_Bot.Utilities;

internal class Miscallenous
{
    public static Color RandomDiscordColour()
    {
        return new Color(new Random().Next(0, 255), new Random().Next(0, 255), new Random().Next(0, 255));
    }
}
