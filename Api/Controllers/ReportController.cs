using Api.Entities;
using Api.Payloads;
using Api.Services;

using FastReport.Export.PdfSimple;
using FastReport.Web;

using Microsoft.AspNetCore.Mvc;

namespace FastReport.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly string ebf23ReportPath;
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
            using MemoryStream stream = labelPrint(ebf23ReportPath, payload);
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
            using MemoryStream stream = labelPrint(ebf40ReportPath, payload);
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
            IEnumerable<Product>? products = DataService.GetProducts(50);

            webReport.Report.Load(productListReportPath);
            webReport.Report.Dictionary.RegisterBusinessObject(
                data: products,
                referenceName: "data",
                maxNestingLevel: products.Count(),
                enabled: true);
            webReport.Report.RegisterData(
                data: products,
                name: "data");
            webReport.Report.Prepare();

            using MemoryStream stream = new();
            webReport.Report.Export(new PDFSimpleExport(), stream);
            stream.Flush();
            return File(stream.ToArray(), "application/pdf", "report.pdf");
        }

        private MemoryStream labelPrint(string reportPath, LabelConfigurationPayload payload)
        {
            //https://fastreports.github.io/FastReport.Documentation/ReferenceReportObject.html
            IEnumerable<Product>? products = DataService.GetProducts(
                payload.Quantity ?? 1,
                payload.JumpFirstLabelsinPaper.GetValueOrDefault());

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
            }

            webReport.Report.Prepare();

            MemoryStream stream = new();
            webReport.Report.Export(new PDFSimpleExport(), stream);
            stream.Flush();

            return stream;
        }
    }
}