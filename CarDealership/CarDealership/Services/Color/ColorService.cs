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

            try
            {
                var existingColor = await _context.Set<CarColor>().FindAsync(editColorViewModel.CarColor.CarColorId);

                if (existingColor == null)
                {
                    return false; // Color not found
                }

                existingColor.Name = editColorViewModel.CarColor.Name; // Update properties as needed

                await _context.SaveChangesAsync();

                return true; // Successfully updated
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CarColorExistsAsync(editColorViewModel.CarColor.CarColorId))
                {
                    return false; // Color not found
                }
                else
                {
                    throw; // Concurrency exception
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new Exception("An error occurred while updating the color.", ex);
            }
        }

        public async Task<bool> CarColorExistsAsync(int id)
        {
            return await _context.CarColors.AnyAsync(e => e.CarColorId == id);
        }
    }
}
