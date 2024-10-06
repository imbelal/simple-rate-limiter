namespace SimpleRateLimiter;

/// <summary>
/// Bucket algorithm for rate limiting.
/// Each user has a bucket of token. A bucket can have maximum N number of tokens.
/// Every second user will get new token until max limit reached.
/// </summary>
public class BucketAlgorithmService
{
    private readonly Dictionary<string, List<int>> _tokenDict;
    private const int MaxNumberOfToken = 10;

    public BucketAlgorithmService()
    {
        _tokenDict = new Dictionary<string, List<int>>();
        // Create a Timer object that calls AddItemToList every 1000 milliseconds (1 second)
        Timer timer = new (AddTokenEverySecond, null, 0, 1000);
    }

    /// <summary>
    /// Add user if it doesn't exists and add MaxNumberOfToken token for the user.
    /// </summary>
    /// <param name="ipAddress"></param>
    public void AddUserWithTokens(string? ipAddress)
    {
        if (string.IsNullOrEmpty(ipAddress) || _tokenDict.ContainsKey(ipAddress)) return;
        
        _tokenDict.Add(ipAddress, new List<int>());
        for (int i = 0; i < MaxNumberOfToken; i++)
        {
            _tokenDict[ipAddress].Add(i);
        }
    }

    /// <summary>
    /// Check if user has token.
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <returns></returns>
    public bool CheckIfUserHasTokens(string ipAddress)
    {
        return _tokenDict.ContainsKey(ipAddress) && _tokenDict[ipAddress].Count > 0;
    }

    /// <summary>
    /// Remove a token if the user has some token.
    /// </summary>
    /// <param name="ipAddress"></param>
    public void RemoveUserWithTokens(string ipAddress)
    {
        if (!_tokenDict.TryGetValue(ipAddress, out List<int>? value)) return;
        value.RemoveAt(value.Count - 1);
    }

    /// <summary>
    /// Add token to every user when user has less than MaxNumberOfToken.
    /// </summary>
    /// <param name="state"></param>
    private void AddTokenEverySecond(object? state)
    {
        foreach (var user in _tokenDict.Where(user => user.Value.Count < MaxNumberOfToken))
        {
            user.Value.Add(0);
        }
    }
}