using Microsoft.AspNetCore.Mvc;
using CarDealership.Data;
using Microsoft.AspNetCore.Authorization;
using CarDealership.Services.Color;
using CarDealership.Models.ColorViewModels;

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
    public async Task<IActionResult> Create(CreateColorViewModel createCarColorViewModel)
    {
        if (ModelState.IsValid)
        {
            await _carColorService.CreateCarColorAsync(createCarColorViewModel);
            return RedirectToAction(nameof(Index));
        }

        return View(createCarColorViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(EditColorViewModel editColorViewModel)
    {
        if (editColorViewModel.Id == null)
        {
            return NotFound();
        }

         editColorViewModel.CarColor = await _carColorService.GetCarColorByIdAsync(editColorViewModel.Id.Value);

        if (editColorViewModel.CarColor == null)
        {
            return NotFound();
        }

        return View(editColorViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(EditColorViewModel editColorViewModel)
    {

        if (ModelState.IsValid)
        {
            var isSuccess = await _carColorService.UpdateCarColorAsync(editColorViewModel);
            if (isSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }

        return View("Edit",editColorViewModel);
    }

    private async Task<bool> ColorExists(int id)
    {
        return await _carColorService.CarColorExistsAsync(id);
    }
}
