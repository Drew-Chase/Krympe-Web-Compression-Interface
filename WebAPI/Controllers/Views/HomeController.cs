using Microsoft.AspNetCore.Mvc;

namespace Krympe.WebAPI.Controllers.Views;

[Route("/")]
public class HomeController : Controller
{
    #region Public Methods

    public IActionResult Index()
    {
        return View();
    }

    #endregion Public Methods
}