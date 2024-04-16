using CarDealership.Models.ModelViewModels;

namespace CarDealership.Services.Model
{
    public interface IModelService
    {
        Task<List<IGrouping<string, Data.Model>>> GetModelsGroupedByBrandAsync();
        Task<List<Data.Brand>> GetAllBrandsAsync();
        Task<bool> CreateModelAsync(CreateModelViewModel createModelViewModel);
        Task<Data.Model> GetModelByIdAsync(int id);
        Task<bool> UpdateModelAsync(EditModelViewModel editModelViewModel);
        Task<bool> ModelExistsAsync(int id);
    }
}
