using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagementSystem.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
