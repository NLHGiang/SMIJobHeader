namespace SMIJobHeader.Model.Message;

public class MessageLogDto
{
    public int ErrorCode { get; set; }
    public string? Messsage { get; set; }
    public int LevelLog { get; set; }
    public string? BodyRequet { get; set; }
}