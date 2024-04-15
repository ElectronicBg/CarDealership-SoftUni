using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarDealership.Data;
using Microsoft.AspNetCore.Authorization;
using CarDealership.Services.Model;

[Authorize(Roles = "Admin")]
public class ModelController : Controller
{
    private readonly IModelService _modelService;

    public ModelController(IModelService modelService)
    {
        _modelService = modelService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var modelsByBrand = await _modelService.GetModelsGroupedByBrandAsync();
        return View(modelsByBrand);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Brands = await _modelService.GetAllBrandsAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Model model)
    {
        if (ModelState.IsValid)
        {
            await _modelService.CreateModelAsync(model);
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Brands = await _modelService.GetAllBrandsAsync();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var model = await _modelService.GetModelByIdAsync(id.Value);
        if (model == null)
        {
            return NotFound();
        }

        ViewBag.Brands = await _modelService.GetAllBrandsAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Model model)
    {
        if (id != model.ModelId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var isSuccess = await _modelService.UpdateModelAsync(id, model);
            if (isSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }

        ViewBag.Brands = await _modelService.GetAllBrandsAsync();
        return View(model);
    }

    private async Task<bool> ModelExists(int id)
    {
        return await _modelService.ModelExistsAsync(id);
    }
}
