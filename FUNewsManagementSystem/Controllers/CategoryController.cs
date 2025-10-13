using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagementSystem.Controllers
{
    public class CategoryController : Controller
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
