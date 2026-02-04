using Microsoft.AspNetCore.Mvc;

namespace OrgWebClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
