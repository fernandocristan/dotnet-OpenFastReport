using Api.Entities;
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
        private readonly string productListReportPath;
        private WebReport webReport;

        public ReportController(IWebHostEnvironment webHostEnvironment)
        {
            productListReportPath = Path.Combine(
                webHostEnvironment.ContentRootPath,
                "wwwroot/reports",
                "ProductList.frx");
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
        /// Simple products report
        /// </summary>
        /// <param name="payload"> </param>
        /// <returns> </returns>
        [HttpGet]
        [Route("product/simple")]
        public ActionResult GetProductsSimpleReport()
        {
            IEnumerable<Product>? products = DataService.GetProducts();

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
    }
}