using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers
{
    public class ArticlesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create() => View();

        public IActionResult Update() => View();

        public IActionResult Delete() => View();
    }
}
