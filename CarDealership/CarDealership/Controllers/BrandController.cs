using CarDealership.Data;
using CarDealership.Models.BrandViewModels;
using CarDealership.Services.Brand;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class BrandController : Controller
{
    private readonly IBrandService _brandService;

    public BrandController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var brands = await _brandService.GetAllBrandsAsync();
        return View(brands);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create( CreateBrandViewModel brandViewModel)
    {
        if (ModelState.IsValid)
        {
            await _brandService.CreateBrandAsync(brandViewModel);
            return RedirectToAction(nameof(Index));
        }
        return View(brandViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(EditBrandViewModel editBrandViewModel)
    {
        if (editBrandViewModel.Id == null)
        {
            return NotFound();
        }

        editBrandViewModel.Brand = await _brandService.GetBrandByIdAsync(editBrandViewModel.Id.Value);
        if (editBrandViewModel.Brand == null)
        {
            return NotFound();
        }

        return View(editBrandViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(EditBrandViewModel editBrandViewModel)
    {

        if (ModelState.IsValid)
        {
            var isSuccess = await _brandService.UpdateBrandAsync(editBrandViewModel);
            if (isSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }
        return View("Edit",editBrandViewModel);
    }

    private async Task<bool> BrandExists(int id)
    {
        return await _brandService.BrandExistsAsync(id);
    }
}
