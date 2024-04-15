using CarDealership.Data;
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
    public async Task<IActionResult> Create(Brand brand)
    {
        if (ModelState.IsValid)
        {
            await _brandService.CreateBrandAsync(brand);
            return RedirectToAction(nameof(Index));
        }
        return View(brand);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var brand = await _brandService.GetBrandByIdAsync(id.Value);
        if (brand == null)
        {
            return NotFound();
        }

        return View(brand);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Brand brand)
    {
        if (id != brand.BrandId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var isSuccess = await _brandService.UpdateBrandAsync(id, brand);
            if (isSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }
        return View(brand);
    }

    private async Task<bool> BrandExists(int id)
    {
        return await _brandService.BrandExistsAsync(id);
    }
}
