using CarDealership.Data;
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int? pageSize, int? pageNumber)
        { 
            var cars= await _carService.IndexAsync();

            // Pagination
            pageSize = pageSize ?? 6; // Default page size is 6
            pageNumber = pageNumber ?? 1; // Default page number is 1

            ViewBag.PageSize = pageSize.Value;
            ViewBag.CurrentPage = pageNumber.Value;
            ViewBag.TotalPages = (int)Math.Ceiling((double)cars.Count() / pageSize.Value);

            var paginatedCars = cars
                .Skip((pageNumber.Value - 1) * pageSize.Value)
                .Take(pageSize.Value)
                .ToList();

            // Pass the paginated cars to the view
            return View(paginatedCars);
        }

        // Add Car
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Brands = _context.Brands.ToList();
            ViewBag.CarColors = _context.CarColors.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Car car, List<string> photos)
        {
            if (ModelState.IsValid)
            {
                var isSuccess = await _carService.CreateCarAsync(car, photos);

                if (isSuccess)
                {
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Car") });
                }
            }

            ViewBag.Brands = _context.Brands.ToList();
            ViewBag.CarColors = _context.CarColors.ToList();
            return Json(new { success = false });
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

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var car = _context.Cars
                .Include(c => c.Brand)
                .Include(c => c.Model)
                .Include(c => c.CarColor)
                .Include(c => c.Photos)
                .FirstOrDefault(c => c.CarId == id);

            if (car == null)
            {
                return NotFound();
            }

            ViewBag.Brands = _context.Brands.ToList();
            ViewBag.CarColors = _context.CarColors.ToList();

            // Populate the Model dropdown based on the selected BrandId
            ViewBag.Models = _context.Models
                .Where(m => m.BrandId == car.BrandId)
                .ToList();

            return View(car);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Car editedCar)
        {
            if (ModelState.IsValid)
            {
                var isSuccess = await _carService.UpdateCarAsync(id, editedCar);

                if (isSuccess)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return NotFound();
                }
            }

            ViewBag.Brands = _context.Brands.ToList();
            ViewBag.CarColors = _context.CarColors.ToList();

            // Repopulate the Model dropdown based on the selected BrandId
            ViewBag.Models = _context.Models
                .Where(m => m.BrandId == editedCar.BrandId)
                .ToList();

            return View("Edit", editedCar);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var isSuccess = await _carService.DeleteCarAsync(id);

            if (isSuccess)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
