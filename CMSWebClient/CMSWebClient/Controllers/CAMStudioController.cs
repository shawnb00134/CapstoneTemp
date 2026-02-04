using Microsoft.AspNetCore.Mvc;

namespace CMSWebClient.Controllers
{
    public class CAMStudioController : Controller
    {
        public IActionResult Studio()
        {
            return View();
        }
    }
}
