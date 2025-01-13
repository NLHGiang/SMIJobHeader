namespace SMIJobHeader.Constants;

public class Enums
{
    public enum CrawlStatus
    {
        Processs,
        Succcessfull,
        Error
    }

    public enum ExportCellStyles
    {
        Default = 0,
        Body = 1,
        Header = 2,
        Footer = 3,
        FooterText = 4,
        BodyNumber = 5,
        FooterNumber = 6,
        BodyQuantity = 7,
        BodyPrice = 8,
        FooterQuantity = 9,
        FooterPrice = 10,
        None = 999
    }
}