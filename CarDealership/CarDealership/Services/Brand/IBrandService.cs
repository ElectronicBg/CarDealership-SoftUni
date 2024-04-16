using CarDealership.Models.BrandViewModels;

namespace CarDealership.Services.Brand
{
    public interface IBrandService
    {
        Task<bool> CreateBrandAsync(CreateBrandViewModel brand);
        Task<Data.Brand> GetBrandByIdAsync(int id);
        Task<bool> UpdateBrandAsync(EditBrandViewModel editBrandViewModel);
        Task<bool> BrandExistsAsync(int id);
        Task<List<Data.Brand>> GetAllBrandsAsync();
    }
}
