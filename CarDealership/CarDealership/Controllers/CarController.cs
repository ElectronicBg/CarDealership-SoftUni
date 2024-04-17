using CarDealership.Data;
using CarDealership.Models;
using CarDealership.Services.Car;
using CarDealership.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarDealership.Controllers
{
    public class CarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICarService _carService;
        public CarController(ApplicationDbContext context, ICarService carService)
        {
            _context = context;
            _carService = carService;
        }       
        [HttpGet]
        public async Task<IActionResult> GetModels(int brandId)
        {
            var models = await _carService.GetModelsByBrandIdAsync(brandId);
            return Json(models);
        }

        //Search Cars
        [HttpGet]
        [Route("Car/Search")]
        public IActionResult Search()
        {
            var model = new SearchViewModel();
            ViewBag.Brands = _context.Brands.ToList();
            ViewBag.CarColors = _context.CarColors.ToList();
            return View(model);
        }

        //Search Cars
        [HttpPost]
        [Route("Car/Search")]
        public async Task<IActionResult> Search(SearchViewModel search, int? pageNumber)
        {

            ViewBag.Brands = _context.Brands.ToList();
            ViewBag.CarColors = _context.CarColors.ToList();

            var searchResult = await _carService.SearchAsync(search);
            var results=searchResult.Cars.ToList();

            // Pagination
            if (search.PageSize == 0)
            {
                search.PageSize = 6;
            }
            else
            {
                search.PageSize = search.PageSize; 
            }
            pageNumber = pageNumber ?? 1;

            var paginatedResults = results
                .Skip((pageNumber.Value - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToList();

            // Set pagination-related ViewBag values
            ViewBag.SearchResults = paginatedResults;
            ViewBag.PageSize = search.PageSize;
            ViewBag.CurrentPage = pageNumber.Value;
            ViewBag.TotalPages = (int)Math.Ceiling((double)results.Count() / search.PageSize);

            ViewBag.SelectedModelName = searchResult.ModelName;

            return View(search);
        }

        //Car Details
        [HttpGet]
        [Route("Car/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var car = await _carService.GetCarDetailsAsync(id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }       
    }
}
