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
            // assign values of start and length from HttpContext
            string start = HttpContext.Request.Form["start"].FirstOrDefault();
            string length = HttpContext.Request.Form["length"].FirstOrDefault();
            //sort and search params
            string sortColumn = HttpContext.Request.Form["sortColumn"].FirstOrDefault();
            string sortColumnDirection = HttpContext.Request.Form["sortDirection"].FirstOrDefault();
            string searchValue = HttpContext.Request.Form["search"].FirstOrDefault();

            var dapperResult = _softwareProducts.GetData(start, length, sortColumn, sortColumnDirection, searchValue);
            return Json(new { recordsFiltered = dapperResult.Size, recordsTotal = dapperResult.Size, data = dapperResult.Results });
        }
    }
}
