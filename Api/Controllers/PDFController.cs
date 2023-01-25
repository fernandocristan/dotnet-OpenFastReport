using System.Data;

using Api.Entities;

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
                EAN13 = "7896541236950",
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

            new Label
            {
                Description = "Óculos Prada Eyewear Collection",
                EAN13 = "7896541236950",
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
            }
        };

        private readonly IWebHostEnvironment webHostEnvironment;

        public PDFController(IWebHostEnvironment webHostEnvironment)

        {
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [Route("report/simple")]
        public ActionResult Get()
        {
            //Create report
            WebReport webReport = new();
            string reportPath = Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot/reports", "ProductList.frx");
            webReport.Report.Load(reportPath);

            //Prepare data
            DataTable dataTable = new();
            dataTable.Columns.Add("Description", typeof(string));
            dataTable.Columns.Add("EAN13", typeof(string));
            dataTable.Columns.Add("Price", typeof(decimal));
            dataTable.Columns.Add("Reference", typeof(string));

            foreach (Label label in labels)
            {
                dataTable.Rows.Add(label);
            }
            webReport.Report.RegisterData(dataTable, "products");

            //Prepare report
            webReport.Report.Prepare();

            using MemoryStream stream = new();
            webReport.Report.Export(new PDFSimpleExport(), stream);
            stream.Flush();
            byte[] arrayReport = stream.ToArray();
            return File(arrayReport, "application/pdf", "report.pdf");
        }

        [HttpGet]
        [Route("data")]
        public ActionResult<IEnumerable<Label>> GetData()
        {
            return Ok(labels);
        }
    }
}