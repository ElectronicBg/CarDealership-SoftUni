using CarDealership.Data;
using Microsoft.EntityFrameworkCore;

namespace CarDealership.Services.Color
{
    public class ColorService:IColorService
    {
        private readonly ApplicationDbContext _context;

        public ColorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CarColor>> GetAllCarColorsAsync()
        {
            return await _context.CarColors.ToListAsync();
        }

        public async Task<bool> CreateCarColorAsync(CarColor carColor)
        {
            _context.CarColors.Add(carColor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CarColor> GetCarColorByIdAsync(int id)
        {
            return await _context.CarColors.FindAsync(id);
        }

        public async Task<bool> UpdateCarColorAsync(int id, CarColor carColor)
        {
            if (id != carColor.CarColorId)
            {
                return false;
            }

            _context.Entry(carColor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CarColorExistsAsync(carColor.CarColorId))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
            return true;
        }

        public async Task<bool> CarColorExistsAsync(int id)
        {
            return await _context.CarColors.AnyAsync(e => e.CarColorId == id);
        }
    }
}
