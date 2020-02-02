using System.Collections.Generic;
using System.Linq;
using DataTableRetriever;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DataTableTester.Controllers
{
    public class HomeController : Controller
    {
        private Retriever _softwareProducts;
        public HomeController(IConfiguration configuration)
        {
            _softwareProducts = new Retriever(configuration.GetConnectionString("SnackDb"), "Products", new List<string> { "Id", "Name", "Description" });
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LoadData()
        {
            //pass the HttpContext to the retriever to hand off the start, length, and search/sort parameters
            var dapperResult = _softwareProducts.GetData(HttpContext);
            //define the draw for the result set from the HttpContext
            return Json(new { draw = HttpContext.Request.Form["draw"].FirstOrDefault(), recordsFiltered = dapperResult.Size, recordsTotal = dapperResult.Size, data = dapperResult.Results });
        }
    }
}
