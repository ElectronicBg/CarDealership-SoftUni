using CarDealership.Data;
using CarDealership.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class PhotoController : Controller
{
    private readonly ApplicationDbContext _context;

    public PhotoController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Photo/Index
    [HttpGet]
    public IActionResult Index()
    {
        var photosByCar = _context.Photos
       .OrderBy(p => p.CarId) // Ensure photos are sorted by CarId before grouping
       .ToList()
       .GroupBy(p => p.CarId); // Group photos by CarId

        return View(photosByCar);
    }

    // GET: Photo/Create
    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Cars = _context.Cars.ToList();
        return View();
    }

    // POST: Photo/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Photo photo)
    {
        if (ModelState.IsValid)
        {
            _context.Photos.Add(photo);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Cars = _context.Cars.ToList();
        return View(photo);
    }

    [HttpPost]
    public IActionResult AddToCar(int carId, List<string> photoUrls)
    {
        if (photoUrls != null && photoUrls.Any())
        {
            foreach (var url in photoUrls)
            {
                var photo = new Photo { CarId = carId, Url = url };
                _context.Photos.Add(photo);
            }

            _context.SaveChanges();
            return Ok(); 
        }

        return BadRequest("No photo URLs provided");
    }

    // POST: Photo/Delete/5
    [HttpPost]
    [Route("Admin/Photo/Delete/{photoId}")]
    public IActionResult Delete(int photoId)  
    {
        var photo = _context.Photos.Find(photoId);

        if (photo == null)
        {
            return NotFound();
        }

        _context.Photos.Remove(photo);
        _context.SaveChanges();

        return Ok();
    }

    private bool PhotoExists(int id)
    {
        return _context.Photos.Any(e => e.PhotoId == id);
    }
}
