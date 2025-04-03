using Microsoft.AspNetCore.Mvc;

namespace HealthRec.Presentation.Controllers;

public class RecordController : Controller
{
    // GET
    public IActionResult Index()
    {
        return this.View();
    }
}