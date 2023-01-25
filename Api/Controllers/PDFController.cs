using Api.Entities;
using Api.Payloads;

using FastReport.Export.PdfSimple;
using FastReport.Web;

using Microsoft.AspNetCore.Mvc;

namespace FastReport.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PDFController : ControllerBase
    {
        private readonly IEnumerable<Label> labels = new List<Label>
        {
            new Label
            {
                Description = "Óculos Prada Eyewear Collection",
                EAN13 = "0106947719224",
                Price = 2356.5m,
                Reference = "7Q98DD"
            },
            new Label
            {
                Description = "Óculos de Sol CARTIER 0229S 002",
                EAN13 = "7890003269871",
                Price = 1999.5m,
                Reference = "13698742556"
            },
            new Label
            {
                Description = "Óculos de sol GUCCI preto injetado",
                EAN13 = "7890006895423",
                Price = 2379,
                Reference = "GG0810S-001 53"
            },
            new Label
            {
                Description = "Metal frame 02 sunglasses in metal with mineral glass lenses gold/green",
                EAN13 = "7896016541236",
                Price = 3500,
                Reference = "AVD3-FGA"
            },
        };

        private readonly IWebHostEnvironment webHostEnvironment;

        public PDFController(IWebHostEnvironment webHostEnvironment)

        {
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        [Route("report/sample")]
        public ActionResult CreateReport()
        {
            //Create report
            WebReport webReport = new();
            string reportPath = Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot/reports", "Sample.frx");

            webReport.Report.Dictionary.RegisterBusinessObject(labels, "data", 10, true);
            webReport.Report.Save(reportPath);

            return Ok($"save in {reportPath}");
        }

        [HttpGet]
        [Route("report/simple")]
        public ActionResult Get()
        {
            //Create report
            WebReport webReport = new();
            string reportPath = Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot/reports", "ProductList.frx");
            webReport.Report.Load(reportPath);
            webReport.Report.Dictionary.RegisterBusinessObject(labels, "data", 0, true);
            webReport.Report.RegisterData(labels, "data");

            //Prepare report
            webReport.Report.Prepare();

            using MemoryStream stream = new();
            webReport.Report.Export(new PDFSimpleExport(), stream);
            stream.Flush();
            return File(stream.ToArray(), "application/pdf", "report.pdf");
        }

        [HttpGet]
        [Route("report/simple/custom-margins")]
        public ActionResult GetCustomMargins([FromQuery] CustomMarginPayload payload)
        {
            //Create report
            WebReport webReport = new();
            string reportPath = Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot/reports", "ProductList.frx");
            webReport.Report.Load(reportPath);
            webReport.Report.Dictionary.RegisterBusinessObject(labels, "data", 0, true);
            webReport.Report.RegisterData(labels, "data");

            foreach (ReportPage page in webReport.Report.Pages)
            {
                page.TopMargin = payload.TopMilimiters.GetValueOrDefault();
                page.BottomMargin = payload.TopMilimiters.GetValueOrDefault();
                page.LeftMargin = payload.LeftMilimiters.GetValueOrDefault();
                page.RightMargin = payload.RightMilimiters.GetValueOrDefault();
            }

            webReport.Report.Prepare();

            using MemoryStream stream = new();
            webReport.Report.Export(new PDFSimpleExport(), stream);
            stream.Flush();
            return File(stream.ToArray(), "application/pdf", "report.pdf");
        }

        [HttpGet]
        [Route("data")]
        public ActionResult<IEnumerable<Label>> GetData()
        {
            return Ok(labels);
        }
    }
}