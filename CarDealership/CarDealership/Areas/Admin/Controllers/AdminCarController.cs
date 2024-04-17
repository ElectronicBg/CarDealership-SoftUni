using CarDealership.Data;
using CarDealership.Models;
using CarDealership.Services.Car;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarDealership.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminCarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICarService _carService;

        public AdminCarController(ApplicationDbContext context, ICarService carService)
        {
            _context = context;
            _carService = carService;
        }
        public async Task<IActionResult> Index(int? pageSize, int? pageNumber)
        {
            var cars = await _carService.IndexAsync();

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
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Brands = _context.Brands.ToList();
            ViewBag.CarColors = _context.CarColors.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCarViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var isSuccess = await _carService.CreateCarAsync(viewModel);

                if (isSuccess)
                {
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "AdminCar") });
                }
            }

            ViewBag.Brands = _context.Brands.ToList();
            ViewBag.CarColors = _context.CarColors.ToList();
            return Json(new { success = false });
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
