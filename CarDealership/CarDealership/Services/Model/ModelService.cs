using CarDealership.Data;
using Microsoft.EntityFrameworkCore;

namespace CarDealership.Services.Model
{
    public class ModelService:IModelService
    {
        private readonly ApplicationDbContext _context;

        public ModelService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<IGrouping<string, Data.Model>>> GetModelsGroupedByBrandAsync()
        {
            var models = await _context.Models
                         .Include(m => m.Brand)
                         .OrderBy(m => m.Brand.Name)
                         .ToListAsync();

            var groupedModels = models.GroupBy(m => m.Brand.Name).ToList();

            return groupedModels;
        }

        public async Task<List<Data.Brand>> GetAllBrandsAsync()
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<bool> CreateModelAsync(Data.Model model)
        {
            _context.Models.Add(model);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Data.Model> GetModelByIdAsync(int id)
        {
            return await _context.Models
                .Include(m => m.Brand)
                .FirstOrDefaultAsync(m => m.ModelId == id);
        }

        public async Task<bool> UpdateModelAsync(int id, Data.Model model)
        {
            if (id != model.ModelId)
            {
                return false;
            }

            _context.Entry(model).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ModelExistsAsync(model.ModelId))
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

        public async Task<bool> ModelExistsAsync(int id)
        {
            return await _context.Models.AnyAsync(e => e.ModelId == id);
        }
    }
}
