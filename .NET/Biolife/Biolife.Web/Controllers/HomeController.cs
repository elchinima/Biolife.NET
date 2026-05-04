using System.Diagnostics;
using Biolife.Web.Models;

namespace Biolife.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? id)
    {
        ViewData["StatusCode"] = id ?? HttpContext.Response.StatusCode;
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
