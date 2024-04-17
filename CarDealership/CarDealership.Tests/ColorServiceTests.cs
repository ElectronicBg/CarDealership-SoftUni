using CarDealership.Data;
using CarDealership.Models.ColorViewModels;
using CarDealership.Services.Color;
using MockQueryable.Moq;
using Moq;

namespace CarDealership.Tests
{
    public class ColorServiceTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly ColorService _carColorService;

        public ColorServiceTests()
        {
            _mockContext = new Mock<ApplicationDbContext>();
            _carColorService = new ColorService(_mockContext.Object);
        }

        [Test]
        public async Task GetAllCarColorsAsync_ReturnsListOfCarColors()
        {
            // Arrange
            var data = new List<CarColor>
        {
            new CarColor { CarColorId = 1, Name = "Red" },
            new CarColor { CarColorId = 2, Name = "Blue" }
        }.AsQueryable();

            var mock = data.BuildMockDbSet();
            _mockContext.Setup(m => m.CarColors).Returns(mock.Object);

            // Act
            var result = await _carColorService.GetAllCarColorsAsync();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Red", result[0].Name);
            Assert.AreEqual("Blue", result[1].Name);
        }

        [Test]
        public async Task CreateCarColorAsync_ReturnsTrueWhenCarColorIsCreated()
        {
            // Arrange
            var createColorViewModel = new CreateColorViewModel { CarColor = new CarColor { CarColorId = 1, Name = "Red" } };

            // Act
            var result = await _carColorService.CreateCarColorAsync(createColorViewModel);

            // Assert
            Assert.True(result);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Test]
        public async Task GetCarColorByIdAsync_ReturnsCarColorWithGivenId()
        {
            // Arrange
            var id = 1;
            var carColor = new CarColor { CarColorId = id, Name = "Red" };
            _mockContext.Setup(m => m.CarColors.FindAsync(id)).ReturnsAsync(carColor);

            // Act
            var result = await _carColorService.GetCarColorByIdAsync(id);

            // Assert
            Assert.AreEqual(carColor, result);
        }

        [Test]
        public async Task UpdateCarColorAsync_ReturnsTrueWhenCarColorIsUpdated()
        {
            // Arrange
            var editColorViewModel = new EditColorViewModel { CarColor = new CarColor { CarColorId = 1, Name = "Red" } };

            // Set up the mock context to return the car color from the database

            _mockContext.Setup(m => m.CarColors.FindAsync(editColorViewModel.CarColor.CarColorId))

                .ReturnsAsync(editColorViewModel.CarColor);

            // Act
            var result = await _carColorService.UpdateCarColorAsync(editColorViewModel);

            // Assert
            Assert.True(result);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Test]
        public async Task CarColorExistsAsync_ReturnsTrueWhenCarColorExists()
        {
            // Arrange
            var id = 1;
            var data = new List<CarColor>
        {
            new CarColor { CarColorId = 1 },
            // Add more sample data if necessary
        }.AsQueryable();

            var mockSet = data.BuildMockDbSet();
            _mockContext.Setup(c => c.CarColors).Returns(mockSet.Object);

            // Act
            var result = await _carColorService.CarColorExistsAsync(id);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task CarColorExistsAsync_ReturnsFalseWhenCarColorDoesNotExist()
        {
            // Arrange
            var id = 1;
            var data = new List<CarColor>
        {
            new CarColor { CarColorId = 2 },
        }.AsQueryable();

            var mockSet = data.BuildMockDbSet();
            _mockContext.Setup(c => c.CarColors).Returns(mockSet.Object);

            // Act
            var result = await _carColorService.CarColorExistsAsync(id);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
