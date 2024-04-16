using CarDealership.Data;
using CarDealership.Models.BrandViewModels;
using Microsoft.EntityFrameworkCore;

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
            _context.Entry(editBrandViewModel.Brand).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await BrandExistsAsync(editBrandViewModel.Brand.BrandId))
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

        public async Task<bool> BrandExistsAsync(int id)
        {
            return await _context.Brands.AnyAsync(e => e.BrandId == id);
        }
    }
}
