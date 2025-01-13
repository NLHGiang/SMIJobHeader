using SMIJobHeader.Extensions;

namespace SMIJobHeader.Utils;

public class MValidationResult
{
    public MValidationResult()
    {
        Members = new List<string>();
    }

    public MValidationResult(int record, string mesage, List<string> members) :
        this()
    {
        Record = record;
        Members = members;
        Message = mesage;
    }

    public MValidationResult(int record, string mesage, string member) :
        this()
    {
        Record = record;
        Message = mesage;
        if (member.IsNotNullOrEmpty()) Members.Add(member);
    }

    public int Record { get; set; }
    public string Message { get; set; }

    public List<string> Members { get; set; }

    public override string ToString()
    {
        var message = Message;
        if (Members == null || !Members.Any())
        {
            message += "\n";
            return message;
        }

        if (Members.Any()) message += $" Nội dung: {string.Join(",", Members)}";
        message += "\n";
        return message;
    }
}