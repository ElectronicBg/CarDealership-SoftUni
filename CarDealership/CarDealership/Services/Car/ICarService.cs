using CarDealership.Helpers;
using CarDealership.ViewModel;
using CarDealership.Data;
using Microsoft.AspNetCore.Mvc;
using CarDealership.Models;

namespace CarDealership.Services.Car
{
    public interface ICarService
    {
        Task<IQueryable<Data.Car>> IndexAsync();
        Task<bool> CreateCarAsync(CreateCarViewModel viewModel);
        Task<IEnumerable<object>> GetModelsByBrandIdAsync(int brandId);
        Task<CarsWithSelectedModelName> SearchAsync(SearchViewModel search);
        Task<Data.Car> GetCarDetailsAsync(int id);
        Task<bool> UpdateCarAsync(int id, Data.Car editedCar);
        Task<bool> DeleteCarAsync(int id);
    }
}
