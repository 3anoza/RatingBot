using TwitchBot.Bot;

namespace TwitchBot;

public class Program
{
    public static void Main(string[] args)
    {
        var channel = Console.ReadLine();
        BotClient bot = new BotClient(channel);
        Console.ReadLine();
    }
}