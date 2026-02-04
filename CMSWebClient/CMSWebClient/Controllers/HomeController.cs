using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMSWebClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // If already authenticated, redirect to dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult PageNotFound()
        {
            return View();
        }
    }
}