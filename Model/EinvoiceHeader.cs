using SMIJobHeader.Entities;

namespace SMIJobHeader.Model;

public class EinvoiceHeader
{
    public string? key { get; set; }
    public string? nbmst { get; set; } = "0309861244";
    public object? khmshdon { get; set; } = 1;
    public string? khhdon { get; set; } = "K24TBN";
    public object? shdon { get; set; } = 1287473;
    public string? cqt { get; set; }
    public Cttkhac[]? cttkhac { get; set; }
    public string? dvtte { get; set; } = "VND";
    public string? hdon { get; set; }
    public string? hsgcma { get; set; }
    public string? hsgoc { get; set; }
    public int? hthdon { get; set; }
    public object? htttoan { get; set; }
    public string? id { get; set; }
    public object? idtbao { get; set; }
    public object? khdon { get; set; }
    public object? khhdgoc { get; set; }
    public object? khmshdgoc { get; set; }
    public object? lhdgoc { get; set; }
    public string? mhdon { get; set; }
    public object? mtdiep { get; set; }
    public string? mtdtchieu { get; set; }
    public string? nbdchi { get; set; }
    public object? nbhdktngay { get; set; }
    public object? nbhdktso { get; set; }
    public object? nbhdso { get; set; }
    public object? nblddnbo { get; set; }
    public object? nbptvchuyen { get; set; }
    public string? nbstkhoan { get; set; }
    public string? nbten { get; set; } = "CÔNG TY ĐIỆN LỰC BẮC TỪ LIÊM";
    public string? nbtnhang { get; set; }
    public object? nbtnvchuyen { get; set; }
    public Nbttkhac[]? nbttkhac { get; set; }
    public DateTime? ncma { get; set; }
    public DateTime? ncnhat { get; set; }
    public string? ngcnhat { get; set; }
    public DateTime? nky { get; set; }
    public string? nmdchi { get; set; }
    public string? nmmst { get; set; }
    public object? nmstkhoan { get; set; }
    public string? nmten { get; set; }
    public object? nmtnhang { get; set; }
    public object? nmtnmua { get; set; }
    public object[]? nmttkhac { get; set; }
    public DateTime? ntao { get; set; }
    public DateTime? ntnhan { get; set; }
    public string? pban { get; set; }
    public object? ptgui { get; set; }
    public object? shdgoc { get; set; }
    public object? tchat { get; set; }
    public object? tdlap { get; set; }
    public object? tgia { get; set; } = 1.0;
    public object? tgtcthue { get; set; } = 11234515.0;
    public object? tgtthue { get; set; } = 898761.0;
    public string? tgtttbchu { get; set; }
    public object? tgtttbso { get; set; }
    public string? thdon { get; set; }
    public object? thlap { get; set; }
    public object[]? thttlphi { get; set; }
    public Thttltsuat[]? thttltsuat { get; set; }
    public string? tlhdon { get; set; }
    public object? ttcktmai { get; set; } = 0;
    public object? tthai { get; set; }
    public Ttkhac[]? ttkhac { get; set; }
    public object? tttbao { get; set; }
    public Ttttkhac[]? ttttkhac { get; set; }
    public object? ttxly { get; set; } = 5;
    public string? tvandnkntt { get; set; }
    public object? mhso { get; set; }
    public object? ladhddt { get; set; }
    public object? mkhang { get; set; }
    public string? nbsdthoai { get; set; }
    public object? nbdctdtu { get; set; }
    public object? nbfax { get; set; }
    public object? nbwebsite { get; set; }
    public string? nbcks { get; set; }
    public object? nmsdthoai { get; set; }
    public object? nmdctdtu { get; set; }
    public object? nmcmnd { get; set; }
    public object? nmcks { get; set; }
    public object? bhphap { get; set; }
    public object? hddunlap { get; set; }
    public object? gchdgoc { get; set; }
    public object? tbhgtngay { get; set; }
    public object? bhpldo { get; set; }
    public object? bhpcbo { get; set; }
    public object? bhpngay { get; set; }
    public object? tdlhdgoc { get; set; }
    public object? tgtphi { get; set; }
    public object? unhiem { get; set; }
    public object? mstdvnunlhdon { get; set; }
    public object? tdvnunlhdon { get; set; }
    public object? nbmdvqhnsach { get; set; }
    public object? nbsqdinh { get; set; }
    public object? nbncqdinh { get; set; }
    public object? nbcqcqdinh { get; set; }
    public object? nbhtban { get; set; }
    public object? nmmdvqhnsach { get; set; }
    public object? nmddvchden { get; set; }
    public object? nmtgvchdtu { get; set; }
    public object? nmtgvchdden { get; set; }
    public object? nbtnban { get; set; }
    public object? dcdvnunlhdon { get; set; }
    public object? dksbke { get; set; }
    public object? dknlbke { get; set; }
    public string? thtttoan { get; set; }
    public string? msttcgp { get; set; }
    public string? cqtcks { get; set; }
    public string? gchu { get; set; }
    public object? kqcht { get; set; }
    public object? hdntgia { get; set; }
    public object? tgtkcthue { get; set; }
    public object? tgtkhac { get; set; }
    public object? nmshchieu { get; set; }
    public object? nmnchchieu { get; set; }
    public object? nmnhhhchieu { get; set; }
    public object? nmqtich { get; set; }
    public object? ktkhthue { get; set; }
    public object? hdhhdvu { get; set; }
    public object? qrcode { get; set; }
    public object? ttmstten { get; set; }
    public object? ladhddtten { get; set; }
    public object? hdxkhau { get; set; }
    public object? hdxkptquan { get; set; }
    public object? hdgktkhthue { get; set; }
    public object? hdonLquans { get; set; }
    public bool? tthdclquan { get; set; }
    public object? pdndungs { get; set; }
    public object? hdtbssrses { get; set; }
    public object? hdTrung { get; set; }
    public object? isHDTrung { get; set; }
}