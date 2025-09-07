namespace ViberGoodMorningBot.Models;

public class ViberMessage
{
    public string Event { get; set; } = string.Empty;
    public Sender? Sender { get; set; }
    public Message? Message { get; set; }
}

public class Sender
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class Message
{
    public string Type { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
public class ViberSettings
{
    public string Url { get; set; } = string.Empty;
    public string AuthToken { get; set; } = string.Empty;
}