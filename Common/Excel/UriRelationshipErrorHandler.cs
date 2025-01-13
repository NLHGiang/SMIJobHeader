using DocumentFormat.OpenXml.Packaging;

namespace SMIJobHeader.Common.Excel;

public class UriRelationshipErrorHandler : RelationshipErrorHandler
{
    public override string Rewrite(Uri partUri, string id, string uri)
    {
        return "https://broken-link";
    }
}