using CarDealership.Helpers;
using CarDealership.ViewModel;
using CarDealership.Data;
using Microsoft.AspNetCore.Mvc;

namespace CarDealership.Services.Car
{
    public interface ICarService
    {
        Task<IQueryable<Data.Car>> IndexAsync();
        Task<CarsWithSelectedModelName> SearchAsync(SearchViewModel search);
        Task<Data.Car> GetCarDetailsAsync(int id);
        Task<bool> UpdateCarAsync(int id, Data.Car editedCar);
        Task<bool> DeleteCarAsync(int id);
    }
}
