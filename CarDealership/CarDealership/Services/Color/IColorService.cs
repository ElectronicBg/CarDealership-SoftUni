using CarDealership.Data;
using CarDealership.Models.ColorViewModels;

namespace CarDealership.Services.Color
{
    public interface IColorService
    {
        Task<List<CarColor>> GetAllCarColorsAsync();
        Task<bool> CreateCarColorAsync(CreateColorViewModel createCarColorViewModel);
        Task<CarColor> GetCarColorByIdAsync(int id);
        Task<bool> UpdateCarColorAsync(EditColorViewModel editColorViewModel);
        Task<bool> CarColorExistsAsync(int id);
    }
}
