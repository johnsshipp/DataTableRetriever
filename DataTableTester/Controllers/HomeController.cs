using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataTableRetriever;
using Microsoft.AspNetCore.Mvc;
using DataTableTester.Models;
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
            //assign values of Draw, start, and length from HttpContext
            var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
            var start = HttpContext.Request.Form["start"].FirstOrDefault();
            var length = HttpContext.Request.Form["length"].FirstOrDefault();
            //sort and search params
            string sortColumn = HttpContext.Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][data]"].FirstOrDefault();
            string sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            string searchValue = Request.Form["search[value]"].FirstOrDefault();

            var dapperResult = _softwareProducts.GetData(start, length, sortColumn, sortColumnDirection, searchValue);
            return Json(new { draw, recordsFiltered = dapperResult.Size, recordsTotal = dapperResult.Size, data = dapperResult.Results });
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
