using CarDealership.Data;
using CarDealership.Helpers;
using CarDealership.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarDealership.Services.Car
{
    public class CarService : ICarService
    {
        private readonly ApplicationDbContext _context;

        public CarService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<IQueryable<Data.Car>> IndexAsync()
        {
            var cars = _context.Cars
                .Include(c => c.Brand)
                .Include(c => c.Model)
                .Include(c => c.CarColor)
                .Include(c => c.Photos)
                .AsQueryable();

            return Task.FromResult(cars);
        }
        public async Task<bool> CreateCarAsync(Data.Car car, List<string> photos)
        {
            if (car == null)
            {
                return false;
            }

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            if (photos != null && photos.Any())
            {
                foreach (var photoUrl in photos)
                {
                    var photoModel = new Photo
                    {
                        CarId = car.CarId,
                        Url = photoUrl
                    };

                    _context.Photos.Add(photoModel);
                }

                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<IEnumerable<object>> GetModelsByBrandIdAsync(int brandId)
        {
            var models = await _context.Models
                .Where(m => m.BrandId == brandId)
                .Select(m => new { m.ModelId, m.Name })
                .ToListAsync();

            return models;
        }
        public async Task<CarsWithSelectedModelName> SearchAsync(SearchViewModel search)
        {
            // Query to get cars based on selected filters
            var query = _context.Cars
                        .Include(c => c.Brand)
                        .Include(c => c.CarColor)
                        .Include(c => c.Model)
                        .Include(c => c.Photos)
                        .AsQueryable();


            // Filter by brand
            if (search.BrandId.HasValue)
            {
                query = query.Where(c => c.BrandId == search.BrandId);
            }

            int? selectedModelId = null;  // Variable to store the selected model ID
            string selectedModelName = null;  // Variable to store the selected model name

            // Filter by model
            if (search.ModelId.HasValue)
            {
                query = query.Where(c => c.ModelId == search.ModelId);
                selectedModelId = search.ModelId;  // Capture the selected model ID

                // Query the database to get the model name based on the selected model ID
                var selectedModel = _context.Models.FirstOrDefault(m => m.ModelId == search.ModelId);
                if (selectedModel != null)
                {
                    selectedModelName = selectedModel.Name;
                }
            }

            // Filter by engine type
            if (search.EngineType.HasValue)
            {
                query = query.Where(c => c.EngineType == search.EngineType);
            }

            // Filter by transmission type
            if (search.TransmissionType.HasValue)
            {
                query = query.Where(c => c.TransmissionType == search.TransmissionType);
            }

            // Filter by color
            if (search.CarColorId.HasValue)
            {
                query = query.Where(c => c.CarColorId == search.CarColorId);
            }

            // Filter by region
            if (search.Region.HasValue)
            {
                query = query.Where(c => c.Region == search.Region);
            }

            // Filter by year range
            if (search.MinYear.HasValue)
            {
                query = query.Where(c => c.Year >= search.MinYear);
            }

            if (search.MaxYear.HasValue)
            {
                query = query.Where(c => c.Year <= search.MaxYear);
            }

            // Filter by car type
            if (search.CarType.HasValue)
            {
                query = query.Where(c => c.CarType == search.CarType);
            }

            // Filter by condition
            if (search.Condition.HasValue)
            {
                query = query.Where(c => c.Condition == search.Condition);
            }

            // Filter by price range
            if (search.MinPrice.HasValue)
            {
                query = query.Where(c => c.Price >= search.MinPrice);
            }

            if (search.MaxPrice.HasValue)
            {
                query = query.Where(c => c.Price <= search.MaxPrice);
            }

            // Apply sorting based on the orderBy parameter
            switch (search.OrderBy)
            {
                case "PriceAsc":
                    query = query.OrderBy(c => c.Price);
                    break;

                case "PriceDesc":
                    query = query.OrderByDescending(c => c.Price);
                    break;

                case "MileageAsc":
                    query = query.OrderBy(c => c.Mileage);
                    break;

                case "MileageDesc":
                    query = query.OrderByDescending(c => c.Mileage);
                    break;

                default:
                    // Default sorting (you can choose a default based on your requirements)
                    query = query.OrderBy(c => c.Price);
                    break;
            }

            // Materialize the query into a list before pagination
            return new CarsWithSelectedModelName
            {
                Cars = await query.ToListAsync(),
                ModelName = selectedModelName
            };
        }
        public async Task<Data.Car> GetCarDetailsAsync(int id)
        {
            var car = await _context.Cars
                .Include(c => c.Brand)
                .Include(c => c.Model)
                .Include(c => c.CarColor)
                .Include(c => c.Photos)
                .FirstOrDefaultAsync(c => c.CarId == id);

            return car;
        }
        public async Task<bool> UpdateCarAsync(int id, Data.Car editedCar)
        {
            if (id != editedCar.CarId)
            {
                return false;
            }

            var existingCar = await _context.Cars
                .FirstOrDefaultAsync(c => c.CarId == id);

            if (existingCar == null)
            {
                return false;
            }

            // Update the existing car entity
            _context.Entry(existingCar).CurrentValues.SetValues(editedCar);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> DeleteCarAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);

            if (car == null)
            {
                return false;
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
