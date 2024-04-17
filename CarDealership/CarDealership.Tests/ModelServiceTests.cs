using CarDealership.Data;
using CarDealership.Models.ModelViewModels;
using CarDealership.Services.Model;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using System.Linq.Expressions;

namespace CarDealership.Tests
{
    public class ModelServiceTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly ModelService _modelService;

        public ModelServiceTests()
        {
            _mockContext = new Mock<ApplicationDbContext>();
            _modelService = new ModelService(_mockContext.Object);
        }

        [Test]
        public async Task GetModelsGroupedByBrandAsync_ReturnsGroupedModels()
        {
            // Arrange
            var brands = new List<Brand>
        {
            new Brand { BrandId = 1, Name = "Brand1" },
            new Brand { BrandId = 2, Name = "Brand2" }
        };
            var models = new List<Model>
        {
            new Model { ModelId = 1, Name = "Model1", Brand = brands[0] },
            new Model { ModelId = 2, Name = "Model2", Brand = brands[1] },
            new Model { ModelId = 3, Name = "Model3", Brand = brands[0] }
        }.AsQueryable();

            _mockContext.Setup(m => m.Models).Returns(models.BuildMockDbSet().Object);

            // Act
            var result = await _modelService.GetModelsGroupedByBrandAsync();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, result[0].Count());
            Assert.AreEqual(1, result[1].Count());
        }

        [Test]
        public async Task GetAllBrandsAsync_ReturnsListOfBrands()
        {
            // Arrange
            var brands = new List<Brand>
        {
            new Brand { BrandId = 1, Name = "Brand1" },
            new Brand { BrandId = 2, Name = "Brand2" }
        }.AsQueryable();

            _mockContext.Setup(m => m.Brands).Returns(brands.BuildMockDbSet().Object);

            // Act
            var result = await _modelService.GetAllBrandsAsync();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Brand1", result[0].Name);
            Assert.AreEqual("Brand2", result[1].Name);
        }

        [Test]
        public async Task CreateModelAsync_ReturnsTrueWhenModelIsCreated()
        {
            // Arrange
            var data = new List<Model>
    {
        new Model { ModelId = 1, Name = "Model1" },
        new Model { ModelId = 2, Name = "Model2" }
    }.AsQueryable();

            var mock = data.BuildMockDbSet();

            _mockContext.Setup(m => m.Models).Returns(mock.Object);

            var createModelViewModel = new CreateModelViewModel
            {
                Model = new Model { ModelId = 3, Name = "Model3" } // Ensure a unique ModelId
            };

            // Act
            var result = await _modelService.CreateModelAsync(createModelViewModel);

            // Assert
            Assert.True(result);
            _mockContext.Verify(m => m.Models.Add(It.IsAny<Model>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Test]
        public async Task UpdateModelAsync_ReturnsTrueWhenModelIsUpdated()
        {
            // Arrange
            var editModelViewModel = new EditModelViewModel { Model = new Model { ModelId = 1, Name = "Model1" } };

            // Act
            var result = await _modelService.UpdateModelAsync(editModelViewModel);

            // Assert
            Assert.True(result);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Test]
        public async Task UpdateModelAsync_ThrowsDbUpdateConcurrencyException_ReturnsFalse()
        {
            // Arrange
            var editModelViewModel = new EditModelViewModel { Model = new Model { ModelId = 1, Name = "Model1" } };

            _mockContext.Setup(m => m.SaveChangesAsync(default)).ThrowsAsync(new DbUpdateConcurrencyException());

            // Act
            var result = await _modelService.UpdateModelAsync(editModelViewModel);

            // Assert
            Assert.False(result);
        }

        [Test]
        public async Task ModelExistsAsync_ReturnsTrueWhenModelExists()
        {
            // Arrange
            var id = 1;
            var data = new List<Model>
        {
            new Model { ModelId = 1 },
            // Add more sample data if necessary
        }.AsQueryable();

            var mockSet = data.BuildMockDbSet();
            _mockContext.Setup(c => c.Models).Returns(mockSet.Object);

            // Act
            var result = await _modelService.ModelExistsAsync(id);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task ModelExistsAsync_ReturnsFalseWhenModelDoesNotExist()
        {
            // Arrange
            var id = 1;
            var data = new List<Model>
        {
            new Model { ModelId = 2 },
        }.AsQueryable();

            var mockSet = data.BuildMockDbSet();
            _mockContext.Setup(c => c.Models).Returns(mockSet.Object);

            // Act
            var result = await _modelService.ModelExistsAsync(id);

            // Assert
            Assert.IsFalse(result);
        }
    }
}

