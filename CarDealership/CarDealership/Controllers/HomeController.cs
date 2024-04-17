using CarDealership.Data;
using CarDealership.Models;
using CarDealership.ViewModel;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CarDealership.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _context;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new SearchViewModel();

            ViewBag.Brands = _context.Brands.ToList();

            return View(model);
        }

        public IActionResult Privacy()
        {
			throw new ArgumentException(nameof(Privacy));
        }

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
			if (exceptionHandlerPathFeature is not null && exceptionHandlerPathFeature.Error is not null)
			{
				_logger.LogError(exceptionHandlerPathFeature.Error, "Unhandled exception.");

			}

			return exceptionHandlerPathFeature switch
			{
				ArgumentException => this.RedirectToAction("Error404"),
				_ => this.RedirectToAction("Error500"),
			};
		}

		[HttpGet]
		public IActionResult Error404()
		{
			return this.View();
		}

		[HttpGet]
		public IActionResult Error500()
		{
			return this.View();
		}

	}
}
