using CarDealership.Helpers;
using CarDealership.ViewModel;

namespace CarDealership.Services.Car
{
    public interface ICarService
    {
        Task<CarsWithSelectedModelName> SearchAsync(SearchViewModel search);
    }
}
