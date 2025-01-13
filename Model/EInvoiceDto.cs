namespace SMIJobHeader.Model;

public class EInvoiceDto
{
    public string MasterKey { get; set; }
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string Type { get; set; }

    public string khmshdon { get; set; } = "";
    public string khhdon { get; set; } = "";
    public string shdon { get; set; } = "";
    public string nbmst { get; set; } = "";
    public string nmmst { get; set; } = "";
    public string nlap { get; set; } = "";
    public string nbten { get; set; } = "";
    public string nbdchi { get; set; } = "";
    public string tgtcthue { get; set; } = "";
    public string tgtthue { get; set; } = "";
    public string ttcktmai { get; set; } = "";
    public string ttphi { get; set; } = "";
    public string tgtttoan { get; set; } = "";
    public string dvtte { get; set; } = "";
    public string tgia { get; set; } = "";

    public int ttxly { get; set; } = 5;
    public string tthoadon { get; set; } = "";
    public string kqkthdon { get; set; } = "";
}