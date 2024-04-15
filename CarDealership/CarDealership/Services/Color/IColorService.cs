using CarDealership.Data;

namespace CarDealership.Services.Color
{
    public interface IColorService
    {
        Task<List<CarColor>> GetAllCarColorsAsync();
        Task<bool> CreateCarColorAsync(CarColor carColor);
        Task<CarColor> GetCarColorByIdAsync(int id);
        Task<bool> UpdateCarColorAsync(int id, CarColor carColor);
        Task<bool> CarColorExistsAsync(int id);
    }
}
