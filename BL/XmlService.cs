using MongoDB.Bson;
using MongoDB.Driver;
using SMIJobXml.BL.Interface;
using SMIJobXml.Constants;
using SMIJobXml.DBAccessor.Interface.Repositories;
using SMIJobXml.Entities;
using SMIJobXml.Extensions;
using SMIJobXml.Model.CrawlData;
using SMIJobXml.Storing;
using System.IO.Compression;

namespace SMIJobXml.BL
{
    public class XmlService : IXmlService
    {
        private readonly ILogger<XmlService> _logger;
        private readonly IAppUnitOfWork _appUOW;
        private readonly IFileService _fileService;
        public XmlService(ILogger<XmlService> logger,
            IAppUnitOfWork appUOW,
            IFileService fileService
        )
        {
            _logger = logger;
            _fileService = fileService;
            _appUOW = appUOW;
        }
        public async Task DispenseXmlMessage(string messageLog)
        {
            var data = messageLog.DeserializeObject<RetryCrawl>();
            var zipBytes = Convert.FromBase64String(data.Result);
            var xmlBytes = GetXMLBytes(zipBytes);
            var invoiceraw = await GetInvoiceraws(data, xmlBytes);
            var invoiceXML = await SaveInvoiceXMl(data, invoiceraw, xmlBytes);
            await UpdateInvoiceraw(invoiceXML);
        }

        private static byte[]? GetXMLBytes(byte[] zipBytes)
        {
            try
            {
                using (var zipStream = new MemoryStream(zipBytes))
                using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                {
                    foreach (var entry in zipArchive.Entries)
                    {
                        if (!entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)) continue;

                        using (var xmlStream = entry.Open())
                        using (var memoryStream = new MemoryStream())
                        {
                            xmlStream.CopyTo(memoryStream);
                            byte[] xmlBytes = memoryStream.ToArray();
                            return xmlBytes;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }

        private async Task<invoiceraws?> GetInvoiceraws(RetryCrawl crawlEInvoice, byte[]? xmlBytes)
        {
            var eInvoice = await GetEInvoiceAsync(crawlEInvoice);
            if (eInvoice == null) return null;
            if (eInvoice.xml != null) return null;

            eInvoice.assets.xml.running = false;
            eInvoice.assets.xml.ran = DateTime.Now;
            var taxCode = crawlEInvoice.InvoiceType.Contains(EInvoiceCrawlConstants.PURCHASE) ? eInvoice.data.nmmst : eInvoice.keys.nbmst;
            bool hasXML = (xmlBytes != null);
            eInvoice.assets.xml.done = hasXML;
            eInvoice.assets.xml.run = hasXML ? eInvoice.assets.xml.run : DateTime.Now.AddMinutes(crawlEInvoice.MinutesAppendCrawlError);
            eInvoice.assets.xml.error_count = hasXML ? eInvoice.assets.xml.error_count : (eInvoice.assets.xml.error_count + 1);

            eInvoice.xml = null;
            if (hasXML)
            {
                eInvoice.xml = new();
                var bucketTime = eInvoice.data.tdlap.FormatDate("yyyyMM");
                if (bucketTime.IsNullOrEmpty())
                {
                    DateTimeExtensions.WriteLogError(eInvoice.SerializeToJson());
                }
                eInvoice.xml.bucketName = hasXML ? $"msmi-{bucketTime}-{taxCode}" : string.Empty;
                eInvoice.xml.pathName = hasXML ? $"{crawlEInvoice.InvoiceType}/query/{eInvoice.keys.nbmst}-{eInvoice.keys.khhdon}-{eInvoice.keys.khmshdon}-{eInvoice.keys.shdon}{FileExtension.XML}" : string.Empty;
            }
            return eInvoice;
        }

        private async Task<invoiceraws?> GetEInvoiceAsync(RetryCrawl crawlEInvoice)
        {
            try
            {
                var repo = _appUOW.GetRepository<invoiceraws, ObjectId>();
                var invoiceType = crawlEInvoice.InvoiceType switch
                {
                    EInvoiceCrawlConstants.PURCHASE => "purchase",
                    EInvoiceCrawlConstants.PURCHASE_SCO => "purchase",
                    EInvoiceCrawlConstants.SOLD => "sold",
                    EInvoiceCrawlConstants.SOLD_SCO => "sold",
                    _ => string.Empty
                };

                var keySearch = $"{crawlEInvoice.User}_{crawlEInvoice.Account}_{invoiceType}_{crawlEInvoice.nbmst}-{crawlEInvoice.khmshdon}-{crawlEInvoice.khhdon}-{crawlEInvoice.shdon}";

                var result = await repo.Get(
                    Builders<invoiceraws>.Filter.Eq(e => e.key, keySearch));
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        private async Task<invoiceraws?> SaveInvoiceXMl(RetryCrawl crawlEInvoice, invoiceraws? invoiceraws, byte[]? xmlBytes)
        {
            if (invoiceraws == null) return null;
            if (xmlBytes == null) return invoiceraws;
            try
            {
                await _fileService.UploadAsync(invoiceraws.xml.bucketName, invoiceraws.xml.pathName, xmlBytes);
            }
            catch (Exception ex)
            {
                invoiceraws.assets.xml.running = false;
                invoiceraws.assets.xml.done = false;
                invoiceraws.assets.xml.run = DateTime.Now.AddMinutes(crawlEInvoice.MinutesAppendCrawlError);
                invoiceraws.assets.xml.ran = DateTime.Now;
                invoiceraws.xml = null;
                invoiceraws.assets.xml.error_count++;
                _logger.LogError(ex.Message);
            }

            return invoiceraws;
        }

        private async Task UpdateInvoiceraw(invoiceraws? item)
        {
            if (item == null) return;

            var repo = _appUOW.GetRepository<invoiceraws, ObjectId>();
            var filter = Builders<invoiceraws>.Filter.Eq(x => x.Id, item.Id);

            var update = Builders<invoiceraws>.Update
                .Set(x => x.assets.xml.running, item.assets.xml.running)
                .Set(x => x.assets.xml.run_count, item.assets.xml.run_count)
                .Set(x => x.assets.xml.error_count, item.assets.xml.error_count)
                .Set(x => x.assets.xml.done, item.assets.xml.done)
                .Set(x => x.assets.xml.run, item.assets.xml.run)
                .Set(x => x.assets.xml.ran, item.assets.xml.ran);

            if (item.xml != null)
            {
                update = update.Set(x => x.xml, item.xml);
            }

            await repo.UpdateAsync(filter, update);
        }

    }
}
