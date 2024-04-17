using CarDealership.Data;
using CarDealership.Models.BrandViewModels;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;

namespace CarDealership.Services.Brand
{
    public class BrandService:IBrandService
    {
        private readonly ApplicationDbContext _context;

        public BrandService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Data.Brand>> GetAllBrandsAsync()
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<bool> CreateBrandAsync(CreateBrandViewModel brand)
        {
            _context.Brands.Add(brand.Brand);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Data.Brand> GetBrandByIdAsync(int id)
        {
            return await _context.Brands.FindAsync(id);
        }

        public async Task<bool> UpdateBrandAsync(EditBrandViewModel editBrandViewModel)
        {
            try
            {
                var existingBrand = await _context.Set<Data.Brand>().FindAsync(editBrandViewModel.Brand.BrandId);

                if (existingBrand == null)
                {
                    return false; // Brand not found
                }

                existingBrand.Name = editBrandViewModel.Brand.Name; // Update properties as needed

                await _context.SaveChangesAsync();

                return true; // Successfully updated
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await BrandExistsAsync(editBrandViewModel.Brand.BrandId))
                {
                    return false; // Brand not found
                }
                else
                {
                    throw; // Concurrency exception
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new Exception("An error occurred while updating the brand.", ex);
            }
        }

        public async Task<bool> BrandExistsAsync(int id)
        {
            return await _context.Brands.AnyAsync(e => e.BrandId == id);
        }
    }
}
