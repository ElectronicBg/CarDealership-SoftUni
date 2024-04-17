using CarDealership.Data;
using CarDealership.Models.BrandViewModels;
using CarDealership.Services.Brand;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

public class BrandServiceTests
{
    private readonly Mock<ApplicationDbContext> _mockContext;
    private readonly BrandService _brandService;

    public BrandServiceTests()
    {
        _mockContext = new Mock<ApplicationDbContext>();
        _brandService = new BrandService(_mockContext.Object);
    }

    [Test]
    public async Task GetAllBrandsAsync_ReturnsListOfBrands()
    {
        // Arrange
        var data = new List<Brand>
            {
                new Brand { BrandId = 1, Name = "Brand1" },
                new Brand { BrandId = 2, Name = "Brand2" }
        }.AsQueryable();

        var mock=data.BuildMockDbSet();

        _mockContext.Setup(m => m.Brands).Returns(mock.Object);

        // Act
        var result = await _brandService.GetAllBrandsAsync();

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].Name, Is.EqualTo("Brand1"));
        Assert.That(result[1].Name, Is.EqualTo("Brand2"));
    }

    [Test]
    public async Task CreateBrandAsync_ReturnsTrueWhenBrandIsCreated()
    {
        // Arrange
        var data = new List<Brand>
            {
                new Brand { BrandId = 1, Name = "Brand1" },
                new Brand { BrandId = 2, Name = "Brand2" }
        }.AsQueryable();

        var mock = data.BuildMockDbSet();

        _mockContext.Setup(m => m.Brands).Returns(mock.Object);

        var brandViewModel = new CreateBrandViewModel
        {
            Brand = new Brand { BrandId = 1, Name = "Brand1" }
        };

        // Act
        var result = await _brandService.CreateBrandAsync(brandViewModel);

        // Assert
        Assert.True(result);
        _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
    }

    [Test]
    public async Task GetBrandByIdAsync_ReturnsBrandWithGivenId()
    {
        // Arrange
        var brand = new Brand { BrandId = 1, Name = "Brand1" };
        _mockContext.Setup(m => m.Brands.FindAsync(brand.BrandId)).ReturnsAsync(brand);

        // Act
        var result = await _brandService.GetBrandByIdAsync(brand.BrandId);

        // Assert
        Assert.That(result, Is.EqualTo(brand));
    } 

    [Test]
    public async Task BrandExistsAsync_ReturnsTrueWhenBrandExists()
    {
        // Arrange
        var id = 1;
        var data = new List<Brand>
    {
        new Brand { BrandId = 1 },
        // Add more sample data if necessary
    }.AsQueryable();

        var mockSet = data.AsQueryable().BuildMockDbSet();

        var mockContext = new Mock<ApplicationDbContext>();
        mockContext.Setup(c => c.Brands).Returns(mockSet.Object);

        var brandService = new BrandService(mockContext.Object);

        // Act
        var result = await brandService.BrandExistsAsync(id);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public async Task BrandExistsAsync_ReturnsFalseWhenBrandDoesNotExist()
    {
        // Arrange
        var id = 1;
        var data = new List<Brand>
        {
            new Brand { BrandId = 2 },
        }.AsQueryable();

        var mockSet = data.AsQueryable().BuildMockDbSet();

        var mockContext = new Mock<ApplicationDbContext>();
        mockContext.Setup(c => c.Brands).Returns(mockSet.Object);

        var brandService = new BrandService(mockContext.Object);

        // Act
        var result = await brandService.BrandExistsAsync(id);

        // Assert
        Assert.False(result);
    }
    [Test]
    public async Task UpdateBrandAsync_ReturnsTrueWhenBrandIsUpdated()
    {
        // Arrange
        var editBrandViewModel = new EditBrandViewModel
        {
            Brand = new Brand { BrandId = 1, Name = "UpdatedBrandName" }
        };

        var existingBrand = new Brand { BrandId = 1, Name = "ExistingBrandName" };

        _mockContext.Setup(m => m.Set<Brand>().FindAsync(editBrandViewModel.Brand.BrandId))
                    .ReturnsAsync(existingBrand);

        _mockContext.Setup(m => m.SaveChangesAsync(default))
                    .ReturnsAsync(1); // Assuming SaveChangesAsync returns 1 for success

        _mockContext.Setup(m => m.Set<Brand>().Update(It.IsAny<Brand>()));

        try
        {
            // Act
            var result = await _brandService.UpdateBrandAsync(editBrandViewModel);

            // Assert
            Assert.True(result);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error message: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            throw;
        }
    }
}

