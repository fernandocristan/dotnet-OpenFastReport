using Api.Entities;
using Api.Payloads;
using Api.Services;

using FastReport.Export.PdfSimple;
using FastReport.Utils;
using FastReport.Web;

using Microsoft.AspNetCore.Mvc;

namespace FastReport.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly string ebf23ReportPath;
        private readonly string ebf40LightReportPath;
        private readonly string ebf40ReportPath;
        private readonly string productListReportPath;
        private WebReport webReport;

        public ReportController(IWebHostEnvironment webHostEnvironment)
        {
            productListReportPath = Path.Combine(
                webHostEnvironment.ContentRootPath,
                "wwwroot/reports",
                "ProductList.frx");
            ebf23ReportPath = Path.Combine(
                webHostEnvironment.ContentRootPath,
                "wwwroot/reports",
                "ebf23.frx");
            ebf40ReportPath = Path.Combine(
                webHostEnvironment.ContentRootPath,
                "wwwroot/reports",
                "ebf40.frx");
            ebf40LightReportPath = Path.Combine(
                webHostEnvironment.ContentRootPath,
                "wwwroot/reports",
                "ebf40_light.frx");
            webReport = new();
        }

        /// <summary>
        /// Its need to create a sample report with BusinessObject datasource linked like this
        /// because in Fortes report community visual editor its not possible
        /// </summary>
        /// <returns> </returns>
        [HttpPost]
        [Route("template-frx")]
        public ActionResult CreateSampleReport()
        {
            const int anyNumber = 10; //Used only to indicate that would have registers

            webReport.Report.Dictionary.RegisterBusinessObject(
                data: new List<Product>(),
                referenceName: "data",
                maxNestingLevel: anyNumber,
                enabled: true);
            webReport.Report.Save(productListReportPath);

            return Ok($"save in {productListReportPath}");
        }

        /// <summary>
        /// Products ebf23 labels report with margins customization
        /// </summary>
        /// <param name="payload"> </param>
        /// <returns> </returns>
        [HttpGet]
        [Route("product/label/ebf23")]
        public ActionResult GetEbf23ProductLabels([FromQuery] LabelConfigurationPayload payload)
        {
            Config.ReportSettings.DefaultPaperSize = DefaultPaperSize.Letter;
            using MemoryStream stream = GetFileStream(ebf23ReportPath, payload);
            return File(stream.ToArray(), "application/pdf", "report.pdf");
        }

        /// <summary>
        /// Products ebf40 labels report with margins customization and whitout product description
        /// </summary>
        /// <param name="payload"> </param>
        /// <returns> </returns>
        [HttpGet]
        [Route("product/label/ebf40/light")]
        public ActionResult GetEbf40LightProductLabels([FromQuery] LabelConfigurationPayload payload)
        {
            Config.ReportSettings.DefaultPaperSize = DefaultPaperSize.Letter;
            using MemoryStream stream = GetFileStream(ebf40LightReportPath, payload);
            return File(stream.ToArray(), "application/pdf", "report.pdf");
        }

        /// <summary>
        /// Products ebf40 labels report with margins customization
        /// </summary>
        /// <param name="payload"> </param>
        /// <returns> </returns>
        [HttpGet]
        [Route("product/label/ebf40")]
        public ActionResult GetEbf40ProductLabels([FromQuery] LabelConfigurationPayload payload)
        {
            Config.ReportSettings.DefaultPaperSize = DefaultPaperSize.Letter;
            using MemoryStream stream = GetFileStream(ebf40ReportPath, payload);
            return File(stream.ToArray(), "application/pdf", "report.pdf");
        }

        /// <summary>
        /// Simple products report
        /// </summary>
        /// <param name="payload"> </param>
        /// <returns> </returns>
        [HttpGet]
        [Route("product/simple")]
        public ActionResult GetProductsListReport()
        {
            Config.ReportSettings.DefaultPaperSize = DefaultPaperSize.A4;
            using MemoryStream stream = GetFileStream(productListReportPath, new LabelConfigurationPayload { Quantity = 50 });
            return File(stream.ToArray(), "application/pdf", "report.pdf");
        }

        private MemoryStream GetFileStream(string reportPath, LabelConfigurationPayload payload)
        {
            //https://fastreports.github.io/FastReport.Documentation/ReferenceReportObject.html
            IEnumerable<Product>? products = DataService.GetProducts(
                payload?.Quantity ?? 1,
                payload?.JumpFirstLabelsinPaper.GetValueOrDefault() ?? 0);

            webReport.Report.Load(reportPath);
            webReport.Report.Dictionary.RegisterBusinessObject(
                data: products,
                referenceName: "data",
                maxNestingLevel: products.Count(),
                enabled: true);
            webReport.Report.RegisterData(
                data: products,
                name: "data");

            foreach (ReportPage page in webReport.Report.Pages)
            {
                page.TopMargin = payload.TopMilimiters ?? page.TopMargin;
                page.BottomMargin = payload.TopMilimiters ?? page.BottomMargin;
                page.LeftMargin = payload.LeftMilimiters ?? page.LeftMargin;
                page.RightMargin = payload.RightMilimiters ?? page.RightMargin;

                //UnitsConverter.ConvertPaperSize("Letter", page); StimulSoft
            }

            webReport.Report.Prepare();

            MemoryStream stream = new();
            webReport.Report.Export(new PDFSimpleExport(), stream);
            stream.Flush();

            return stream;
        }
    }
}