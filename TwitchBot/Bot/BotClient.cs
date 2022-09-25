using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;

namespace TwitchBot.Bot;

public class BotClient
{
    private TwitchClient _client;
    private ChatMarksParser _marksCounter;

    private bool _isListeningChat;

    private string _channelName;

    public BotClient(string channelName)
    {
        _channelName = channelName;

        WebSocketClient socket = new WebSocketClient();

        _marksCounter = new ChatMarksParser();

        _client = new TwitchClient(socket);
        _client.Initialize(GetCredentials(), _channelName);
        _client.OnLog += ClientOnLog;
        _client.OnChatCommandReceived += ClientOnChatCommandReceived;

        _client.Connect();
    }

    private void ClientOnLog(object? sender, OnLogArgs e)
    {
        Console.WriteLine($"[{e.DateTime}]{e.BotUsername}: \n{e.Data}");
    }

    private void ClientOnChatCommandReceived(object? sender, OnChatCommandReceivedArgs e)
    {
        if (!string.Equals(e.Command.ChatMessage.Username, _channelName, StringComparison.CurrentCultureIgnoreCase)) return;

        switch (e.Command.CommandText.ToLower())
        {
            case "startrating":
                if (_isListeningChat) return;

                _client.SendMessage(e.Command.ChatMessage.Channel, $"НАЧАЛО ГОЛОСОВАНИЯ. СТАВЬТЕ ОЦЕНКИ ОТ 1 ДО 10 ");
                _isListeningChat = true;
                _client.OnMessageReceived += ClientOnOnMessageReceived;

                break;
            case "endrating":
                if (!_isListeningChat) return;

                _isListeningChat = false;
                _client.OnMessageReceived -= ClientOnOnMessageReceived;
                
                string avrgMark = $"{_marksCounter.GetAverage():0.0}";
                
                _client.SendMessage(e.Command.ChatMessage.Channel, 
                    $"ГОЛОСОВАНИЕ ЗАКРЫТО. СРЕДНЯЯ ОЦЕНКА: {avrgMark.Replace(',', '.')}; ВСЕГО ГОЛОСОВАЛО: {_marksCounter.GetUsers()}; ТОП ОЦЕНКА: {_marksCounter.GetTopMark()} ( ПРОГОЛОСОВАЛО {_marksCounter.GetSizeOfBestMarks()} )");
                _marksCounter.Clear();
                break;
        }
    }

    private void ClientOnOnMessageReceived(object? sender, OnMessageReceivedArgs e)
    {
        _marksCounter.AddMark(e.ChatMessage.UserId, e.ChatMessage.Message);
    }

    private ConnectionCredentials GetCredentials()
    {
        return new ConnectionCredentials(BotConfig.BOT_NAME, BotConfig.OAUTH_TOKEN);
    }

}