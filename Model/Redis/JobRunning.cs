namespace SMIJobHeader.Model.Redis;

public class JobRunning
{
    public string? JobName { get; set; }
    public bool IsRunning { get; set; }
    public DateTime? LastRun { get; set; }
    public DateTime? TimeWillBeFinish { get; set; }
}