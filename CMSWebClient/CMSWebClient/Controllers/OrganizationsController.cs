using Microsoft.AspNetCore.Mvc;

namespace CMSWebClient.Controllers
{
    public class OrganizationsController : Controller
    {
        public IActionResult OrganizationPage()
        {
            return View();
        }
    }
}
