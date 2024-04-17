using CarDealership.Data;
using CarDealership.Data.Enums;
using CarDealership.Models;
using CarDealership.Services.Car;
using CarDealership.ViewModel;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using System.Linq.Expressions;

namespace CarDealership.Tests
{
    public class CarServiceTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly CarService _carService;

        public CarServiceTests()
        {
            _mockContext = new Mock<ApplicationDbContext>();
            _carService = new CarService(_mockContext.Object);
        }
        [Test]
        public async Task IndexAsync_ReturnsAllCarsWithIncludes()
        {
            // Arrange
            var cars = new List<Car>
        {
            new Car { CarId = 1, BrandId = 1, ModelId = 1, CarColorId = 1 },
            new Car { CarId = 2, BrandId = 1, ModelId = 2, CarColorId = 2 }
        }.AsQueryable();

            _mockContext.Setup(m => m.Cars)
                        .Returns(cars.BuildMockDbSet().Object);

            // Act
            var result = await _carService.IndexAsync();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count());

            var firstCar = result.First();
            Assert.AreEqual(1, firstCar.CarId);
            Assert.AreEqual(1, firstCar.BrandId);
            Assert.AreEqual(1, firstCar.ModelId);
            Assert.AreEqual(1, firstCar.CarColorId);
        }
        
        [Test]
        public async Task CreateCarAsync_ReturnsTrueWhenCarIsCreated()
        {
            // Arrange
            var viewModel = new CreateCarViewModel
            {
                Car = new Car { CarId = 1, BrandId = 1, ModelId = 1 },
                Photos = new List<string> { "photo1.jpg", "photo2.jpg" }
            };

            // Setup DbSet and Add methods for Cars and Photos
            var mockCars = new List<Car>().AsQueryable().BuildMockDbSet();
            var mockPhotos = new List<Photo>().AsQueryable().BuildMockDbSet();

            _mockContext.Setup(m => m.Cars).Returns(mockCars.Object);
            _mockContext.Setup(m => m.Photos).Returns(mockPhotos.Object);

            _mockContext.Setup(m => m.SaveChangesAsync(default))
                        .ReturnsAsync(1); // Assuming 1 is the number of affected rows

            // Act
            var result = await _carService.CreateCarAsync(viewModel);

            // Assert
            Assert.True(result);
            _mockContext.Verify(m => m.Cars.Add(It.IsAny<Car>()), Times.Once);
            _mockContext.Verify(m => m.Photos.Add(It.IsAny<Photo>()), Times.Exactly(2));
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Exactly(2));
        }
        
        [Test]
        public async Task GetModelsByBrandIdAsync_ReturnsModelsByBrandId()
        {
            // Arrange
            var brandId = 1;
            var models = new List<Model>
    {
        new Model { ModelId = 1, Name = "Model1", BrandId = brandId },
        new Model { ModelId = 2, Name = "Model2", BrandId = brandId }
    }.AsQueryable();

            _mockContext.Setup(m => m.Models).Returns(models.BuildMockDbSet().Object);

            // Act
            var result = await _carService.GetModelsByBrandIdAsync(brandId);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().GetType().GetProperty("ModelId").GetValue(result.First()), Is.EqualTo(1));
            Assert.That(result.First().GetType().GetProperty("Name").GetValue(result.First()), Is.EqualTo("Model1"));
        }
        [Test]
        public async Task UpdateCarAsync_ReturnsFalseWhenIdMismatch()
        {
            // Arrange
            var id = 1;
            var editedCar = new Data.Car { CarId = 2, BrandId = 1, ModelId = 1 };

            // Act
            var result = await _carService.UpdateCarAsync(id, editedCar);

            // Assert
            Assert.IsFalse(result);
        }     

        [Test]
        public async Task DeleteCarAsync_ReturnsTrueWhenCarIsDeleted()
        {
            // Arrange
            var id = 1;
            _mockContext.Setup(m => m.Cars.FindAsync(id)).ReturnsAsync(new Car { CarId = id });

            // Act
            var result = await _carService.DeleteCarAsync(id);

            // Assert
            Assert.True(result);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }
        [Test]
        public async Task DeleteCarAsync_ReturnsFalseWhenCarDoesNotExist()
        {
            // Arrange
            var id = 1;
            _mockContext.Setup(m => m.Cars.FindAsync(id)).ReturnsAsync((Car)null); // Return null to simulate car not found

            // Act
            var result = await _carService.DeleteCarAsync(id);

            // Assert
            Assert.False(result);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never); // Verify that SaveChangesAsync was not called
        }
        [Test]
        public async Task SearchAsync_FiltersByBrand_ReturnsFilteredCars()
        {
            // Arrange
            var brandId = 1;
            var searchViewModel = new SearchViewModel { BrandId = brandId };

            var cars = new List<Data.Car>
    {
        new Data.Car { CarId = 1, BrandId = brandId, ModelId = 1 },
        new Data.Car { CarId = 2, BrandId = brandId, ModelId = 2 },
        new Data.Car { CarId = 3, BrandId = 2, ModelId = 1 }  // Different brand
    }.AsQueryable();

            var mock = cars.BuildMockDbSet();
            _mockContext.Setup(m => m.Cars).Returns(mock.Object);

            // Act
            var result = await _carService.SearchAsync(searchViewModel);

            // Assert
            Assert.AreEqual(2, result.Cars.Count()); // Expecting 2 cars with the specified brand
            Assert.IsTrue(result.Cars.All(c => c.BrandId == brandId)); // All cars should have the specified brand
        }
    }
}
