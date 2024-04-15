using Microsoft.AspNetCore.Mvc;
using CarDealership.Data;
using Microsoft.AspNetCore.Authorization;
using CarDealership.Services.Color;

[Authorize(Roles = "Admin")]
public class ColorController : Controller
{
    private readonly IColorService _carColorService;

    public ColorController(IColorService carColorService)
    {
        _carColorService = carColorService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var colors = await _carColorService.GetAllCarColorsAsync();
        return View(colors);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CarColor carColor)
    {
        if (ModelState.IsValid)
        {
            await _carColorService.CreateCarColorAsync(carColor);
            return RedirectToAction(nameof(Index));
        }

        return View(carColor);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var carColor = await _carColorService.GetCarColorByIdAsync(id.Value);

        if (carColor == null)
        {
            return NotFound();
        }

        return View(carColor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CarColor carColor)
    {
        if (id != carColor.CarColorId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var isSuccess = await _carColorService.UpdateCarColorAsync(id, carColor);
            if (isSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }

        return View(carColor);
    }

    private async Task<bool> ColorExists(int id)
    {
        return await _carColorService.CarColorExistsAsync(id);
    }
}
