using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarDealership.Data;
using Microsoft.AspNetCore.Authorization;
using CarDealership.Services.Model;
using CarDealership.Models.ModelViewModels;

[Area("Admin")]
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
    public async Task<IActionResult> Create(CreateModelViewModel createModelViewModel)
    {
        if (ModelState.IsValid)
        {
            await _modelService.CreateModelAsync(createModelViewModel);
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Brands = await _modelService.GetAllBrandsAsync();
        return View(createModelViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(EditModelViewModel editModelViewModel)
    {
        if (editModelViewModel.Id == null)
        {
            return NotFound();
        }

        editModelViewModel.Model = await _modelService.GetModelByIdAsync(editModelViewModel.Id.Value);
        if (editModelViewModel.Model == null)
        {
            return NotFound();
        }

        ViewBag.Brands = await _modelService.GetAllBrandsAsync();
        return View("Edit",editModelViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(EditModelViewModel editModelViewModel)
    {
        if (ModelState.IsValid)
        {
            var isSuccess = await _modelService.UpdateModelAsync(editModelViewModel);
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
        return View(editModelViewModel);
    }

    private async Task<bool> ModelExists(int id)
    {
        return await _modelService.ModelExistsAsync(id);
    }
}
