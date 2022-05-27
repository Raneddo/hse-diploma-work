namespace ADSD.Backend.App.Json;

public class InfoResponse
{
    public InfoResponse(string key, string text)
    {
        Key = key;
        Text = text;
    }

    public string Key { get; }
    public string Text { get; }
}