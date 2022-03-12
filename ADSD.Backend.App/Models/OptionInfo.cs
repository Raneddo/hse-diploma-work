namespace ADSD.Backend.App.Models;

public class OptionInfo
{
    public OptionInfo(int id, int pollId, string optionText, int count, bool isChosen)
    {
        Id = id;
        PollId = pollId;
        OptionText = optionText;
        Count = count;
        IsChosen = isChosen;
    }

    public int Id { get; }
    public int PollId { get; }
    public string OptionText { get; }
    public int Count { get; }
    public bool IsChosen { get; }
}