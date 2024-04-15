namespace CarDealership.Services.Model
{
    public interface IModelService
    {
        Task<List<IGrouping<string, Data.Model>>> GetModelsGroupedByBrandAsync();
        Task<List<Data.Brand>> GetAllBrandsAsync();
        Task<bool> CreateModelAsync(Data.Model model);
        Task<Data.Model> GetModelByIdAsync(int id);
        Task<bool> UpdateModelAsync(int id, Data.Model model);
        Task<bool> ModelExistsAsync(int id);
    }
}
