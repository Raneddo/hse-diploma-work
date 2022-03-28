using ADSD.Backend.App.Clients;
using ADSD.Backend.App.Json;

namespace ADSD.Backend.App.Services;

public class InfoService
{
    private readonly AppDbClient _appDbClient;

    public InfoService(AppDbClient appDbClient)
    {
        _appDbClient = appDbClient;
    }

    public InfoJson GetInfoByKey(string key)
    {
        return _appDbClient.GetInfoByKey(key);
    }

    public void UpdateOrInsertInfo(string key, string text)
    {
        _appDbClient.MergeInfo(key, text);
    }
}