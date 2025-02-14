using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using SMIJobHeader.Entities.Interfaces;

namespace SMIJobHeader.Entities;

[BsonIgnoreExtraElements]
public class invoiceraws : IBaseEntity<ObjectId>
{
    public ObjectId? user { get; set; }
    public ObjectId? account { get; set; }
    public string? from { get; set; }
    public string? type { get; set; }
    public string? key { get; set; }
    public Keys? keys { get; set; }
    public Data? data { get; set; }

    public Header? header { get; set; }

    //public long? priority { get; set; }
    //public Warning? warning { get; set; }
    public Assets? assets { get; set; }

    //public object[]? tags { get; set; }
    //public string? created_by { get; set; }
    //public Sync_Hdtbssrses sync_hdtbssrses { get; set; }
    //public DateTime? last_updated_date { get; set; }
    //public DateTime? created_date { get; set; }
    //public long? __v { get; set; }
    //public Detail? detail { get; set; }
    public string? detail_id { get; set; }
    public Xml? xml { get; set; }

    [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
    public ObjectId Id { get; set; }
}

[BsonIgnoreExtraElements]
public class Keys
{
    public string? nbmst { get; set; }
    public string? khhdon { get; set; }
    public object? shdon { get; set; }
    public object? khmshdon { get; set; }
}

[BsonIgnoreExtraElements]
public class Data
{
    public string? id { get; set; }
    public object? tthai { get; set; }
    public object? ttxly { get; set; }
    public DateTime? tdlap { get; set; }
    public string? nmmst { get; set; }
    public string? nmten { get; set; }
    public string? nbten { get; set; }
}

[BsonIgnoreExtraElements]
public class Header
{
    public string? nbmst { get; set; }
    public object? khmshdon { get; set; }
    public string? khhdon { get; set; }
    public object? shdon { get; set; }
    public string? cqt { get; set; }
    public Cttkhac[]? cttkhac { get; set; }
    public string? dvtte { get; set; }
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
    public string? nbten { get; set; }
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
    public DateTime? tdlap { get; set; }
    public object? tgia { get; set; }
    public object? tgtcthue { get; set; }
    public object? tgtthue { get; set; }
    public string? tgtttbchu { get; set; }
    public object? tgtttbso { get; set; }
    public string? thdon { get; set; }
    public object? thlap { get; set; }
    public object[]? thttlphi { get; set; }
    public Thttltsuat[]? thttltsuat { get; set; }
    public string? tlhdon { get; set; }
    public object? ttcktmai { get; set; }
    public object? tthai { get; set; }
    public Ttkhac[]? ttkhac { get; set; }
    public object? tttbao { get; set; }
    public Ttttkhac[]? ttttkhac { get; set; }
    public object? ttxly { get; set; }
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

[BsonIgnoreExtraElements]
public class Cttkhac
{
    public string? ttruong { get; set; }
    public string? kdlieu { get; set; }
    public string? dlieu { get; set; }
}

[BsonIgnoreExtraElements]
public class Nbttkhac
{
    public string? ttruong { get; set; }
    public string? kdlieu { get; set; }
    public string? dlieu { get; set; }
}

[BsonIgnoreExtraElements]
public class Thttltsuat
{
    public string? tsuat { get; set; }
    public object? thtien { get; set; }
    public object? tthue { get; set; }
    public object? gttsuat { get; set; }
}

[BsonIgnoreExtraElements]
public class Ttkhac
{
    public string? ttruong { get; set; }
    public string? kdlieu { get; set; }
    public string? dlieu { get; set; }
}

[BsonIgnoreExtraElements]
public class Ttttkhac
{
    public string? ttruong { get; set; }
    public string? kdlieu { get; set; }
    public string? dlieu { get; set; }
}

[BsonIgnoreExtraElements]
public class Warning
{
    public string? status { get; set; }
    public string? origin_status { get; set; }
    public object? score { get; set; }
    public object? nkhdhle { get; set; }
    public object? nkhdtnlap { get; set; }
    public object? ttcks { get; set; }
    public object? xmlnven { get; set; }
    public object? cqtcma { get; set; }
    public object? cqtcmatnky { get; set; }
    public object? nbdksd { get; set; }
    public object? dcma { get; set; }
    public DateTime? date { get; set; }
    public object[]? warning_array { get; set; }
    public bool? is_sco { get; set; }
    public bool? ignone_nbcks { get; set; }
    public bool? has_xml { get; set; }
    public object? owner_match { get; set; }
    public string? owner_name { get; set; }
}

[BsonIgnoreExtraElements]
public class Assets
{
    public AssetDetail? detail { get; set; }
    public AssetZip? zip { get; set; }
    public AssetPdf? pdf { get; set; }
    public AssetXml? xml { get; set; }
}

[BsonIgnoreExtraElements]
public class AssetDetail
{
    public bool? running { get; set; }
    public int? run_count { get; set; }
    public int? error_count { get; set; }
    public bool? done { get; set; }
    public DateTime? run { get; set; }
    public DateTime? ran { get; set; }
}

[BsonIgnoreExtraElements]
public class AssetZip
{
    public bool? running { get; set; }
    public int? run_count { get; set; }
    public int? error_count { get; set; }
    public bool? done { get; set; }
    public DateTime? run { get; set; }
}

[BsonIgnoreExtraElements]
public class AssetPdf
{
    public bool? running { get; set; }
    public int? run_count { get; set; }
    public int? error_count { get; set; }
    public bool? done { get; set; }
    public DateTime? run { get; set; }
}

[BsonIgnoreExtraElements]
public class AssetXml
{
    public bool? running { get; set; }
    public int? run_count { get; set; }
    public int? error_count { get; set; }
    public bool? done { get; set; }
    public DateTime? run { get; set; }
    public DateTime? ran { get; set; }
}

[BsonIgnoreExtraElements]
public class Sync_Hdtbssrses
{
    public bool? running { get; set; }
    public object? run_count { get; set; }
    public object? error_count { get; set; }
    public DateTime? run { get; set; }
}

[BsonIgnoreExtraElements]
public class Detail
{
    public string? nbmst { get; set; }
    public object? khmshdon { get; set; }
    public string? khhdon { get; set; }
    public object? shdon { get; set; }
    public string? cqt { get; set; }
    public Cttkhac[]? cttkhac { get; set; }
    public string? dvtte { get; set; }
    public string? hdon { get; set; }
    public string? hsgcma { get; set; }
    public string? hsgoc { get; set; }
    public object? hthdon { get; set; }
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
    public string? nbten { get; set; }
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
    public DateTime? tdlap { get; set; }
    public object? tgia { get; set; }
    public object? tgtcthue { get; set; }
    public object? tgtthue { get; set; }
    public string? tgtttbchu { get; set; }
    public object? tgtttbso { get; set; }
    public string? thdon { get; set; }
    public object? thlap { get; set; }
    public object[]? thttlphi { get; set; }
    public Thttltsuat[]? thttltsuat { get; set; }
    public string? tlhdon { get; set; }
    public object? ttcktmai { get; set; }
    public object? tthai { get; set; }
    public Ttkhac[]? ttkhac { get; set; }
    public object? tttbao { get; set; }
    public Ttttkhac[]? ttttkhac { get; set; }
    public object? ttxly { get; set; }
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
    public Hdhhdvu[]? hdhhdvu { get; set; }
    public string? qrcode { get; set; }
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

[BsonIgnoreExtraElements]
public class Hdhhdvu
{
    public string? idhdon { get; set; }
    public string? id { get; set; }
    public object? dgia { get; set; }
    public string? dvtinh { get; set; }
    public string? ltsuat { get; set; }
    public object? sluong { get; set; }
    public object? stbchu { get; set; }
    public object? stckhau { get; set; }
    public object? stt { get; set; }
    public object? tchat { get; set; }
    public string? ten { get; set; }
    public object? thtcthue { get; set; }
    public object? thtien { get; set; }
    public object? tlckhau { get; set; }
    public float? tsuat { get; set; }
    public object? tthue { get; set; }
    public object? sxep { get; set; }
    public Ttkhac[]? ttkhac { get; set; }
    public object? dvtte { get; set; }
    public object? tgia { get; set; }
}

[BsonIgnoreExtraElements]
public class Xml
{
    public string? bucketName { get; set; }
    public string? pathName { get; set; }
}