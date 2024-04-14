namespace CarDealership.Services.Brand
{
    public interface IBrandService
    {
        Task<bool> CreateBrandAsync(Data.Brand brand);
        Task<Data.Brand> GetBrandByIdAsync(int id);
        Task<bool> UpdateBrandAsync(int id,Data.Brand brand);
        Task<bool> BrandExistsAsync(int id);
        Task<List<Data.Brand>> GetAllBrandsAsync();
    }
}
