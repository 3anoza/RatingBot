namespace TwitchBot;

public class ChatMarksCounter
{
    private Dictionary<string, int> _usersIdList = new Dictionary<string, int>();

    private int _uniqUsersCounter = 0;
    private float _globalMark = 0;

    private int _bestSize = 1;
    private int _bestMark = 0;
    public int GetUsers()
    {
        return _uniqUsersCounter;
    }

    public int GetTopMark()
    {
        if (_usersIdList.Count < 2) return _usersIdList.Last().Value;
        var groups = _usersIdList.GroupBy(u => u.Value).Where(g => g.Count() > 1);

        _bestSize = groups.Select(g => g.Count()).Max();
        _bestMark = groups.Where(g => g.Count() == _bestSize).ToList().FirstOrDefault()!.Key;

        return _bestMark;
    }

    public int GetSizeOfBestMarks()
    {
        if (_usersIdList.Count < 2) return 1;
        return _bestSize;
    }

    public float GetAverage()
    {
        foreach (var user in _usersIdList)
        {
            _globalMark += user.Value;
        }

        if (_globalMark == 0)
            return 0;
        if (_uniqUsersCounter == 0)
            return 0;

        return _globalMark / _uniqUsersCounter;
    }

    public void Clear()
    {
        _usersIdList.Clear();
    }

    public void AddMark(string userId, string rawMessage)
    {
        if (!int.TryParse(rawMessage, out int mark)) return;
        if (mark is > 10 or < 1) return;



        if (_usersIdList.ContainsKey(userId))
        {
            _usersIdList.Add(userId+_uniqUsersCounter, mark);
            _uniqUsersCounter++;
        }
        else
        {
            _usersIdList.Add(userId, mark);
            _uniqUsersCounter++;
        }
    }
}