using Microsoft.AspNetCore.Mvc;

namespace CMSWebClient.Controllers
{
    public class PeopleController : Controller
    {
        public IActionResult PeopleHome()
        {
            return View();
        }
    }
}
