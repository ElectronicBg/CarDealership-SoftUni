using CarDealership.Data;
using CarDealership.Models.ColorViewModels;
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

        public async Task<bool> CreateCarColorAsync(CreateColorViewModel createCarColorViewModel)
        {
            _context.CarColors.Add(createCarColorViewModel.CarColor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CarColor> GetCarColorByIdAsync(int id)
        {
            return await _context.CarColors.FindAsync(id);
        }

        public async Task<bool> UpdateCarColorAsync(EditColorViewModel editColorViewModel)
        {

            _context.Entry(editColorViewModel.CarColor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CarColorExistsAsync(editColorViewModel.CarColor.CarColorId))
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
