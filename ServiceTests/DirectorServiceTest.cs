using ClassLibrary1.Dtos;
using ClassLibrary1.RepositoryInterfaces;
using ClassLibrary1.Responses;
using ClassLibrary1.ServiceImplementation;
using Domain.Exceptions;
using Domain.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace MovieServiceTests;

public class DirectorServiceTest
{
    [Fact]
    public async Task AddDirectorAsync_ValidDirectorDto_ReturnsDirectorResponse()
    {
        // Arrange
        var directorDto = new DirectorDto
        {
            DirectorName = "DirectorName"
        };

        Guid.NewGuid();

        var directorRepositoryMock = new Mock<IDirectorRepository>();
        var directorService = new DirectorService(directorRepositoryMock.Object);

        // Act
        var result = await directorService.AddDirectorAsync(directorDto);

        // Assert
        result.Should().NotBeNull().And.BeOfType<DirectorResponse>();
    }

    [Fact]
    public async Task AddDirectorAsync_InvalidDirectorDto_ThrowsMultiValidationException()
    {
        // Arrange
        var directorDto = new DirectorDto
        {
            DirectorName = string.Empty
        };

        var directorRepositoryMock = new Mock<IDirectorRepository>();
        var directorService = new DirectorService(directorRepositoryMock.Object);

        // Act and Assert
        await Assert.ThrowsAsync<MultiValidationException>(async () =>
        {
            await directorService.AddDirectorAsync(directorDto);
        });
    }

    [Fact]
    public async Task GetByIdAsync_ExistingDirectorId_ReturnsDirectorResponse()
    {
        // Arrange
        var directorId = Guid.NewGuid();
        var expectedDirector = new Director(directorId, "DirectorName");

        var directorRepositoryMock = new Mock<IDirectorRepository>();
        var directorService = new DirectorService(directorRepositoryMock.Object);

        directorRepositoryMock
            .Setup(repo => repo.GetByIdAsync(directorId))
            .ReturnsAsync(expectedDirector);

        // Act
        var result = await directorService.GetByIdAsync(directorId);

        // Assert
        result.Should().NotBeNull().And.BeOfType<DirectorResponse>();
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingDirectorId_ThrowsMultiValidationException()
    {
        // Arrange
        var directorId = Guid.NewGuid();

        var directorRepositoryMock = new Mock<IDirectorRepository>();
        var directorService = new DirectorService(directorRepositoryMock.Object);

        directorRepositoryMock
            .Setup(repo => repo.GetByIdAsync(directorId))
            .ReturnsAsync((Director)null!);

        // Act and Assert
        await Assert.ThrowsAsync<MultiValidationException>(async () =>
        {
            await directorService.GetByIdAsync(directorId);
        });
    }

    [Fact]
    public async Task DeleteDirectorAsync_ExistingDirectorId_DeletesDirector()
    {
        // Arrange
        var directorId = Guid.NewGuid();

        var directorRepositoryMock = new Mock<IDirectorRepository>();
        var directorService = new DirectorService(directorRepositoryMock.Object);

        directorRepositoryMock
            .Setup(repo => repo.GetByIdAsync(directorId))
            .ReturnsAsync(new Director { Id = directorId });

        directorRepositoryMock
            .Setup(repo => repo.DeleteDirectorAsync(directorId))
            .ReturnsAsync(true);

        // Act
        await directorService.DeleteDirectorAsync(directorId);

        // Assert
        directorRepositoryMock.Verify(repo => repo.DeleteDirectorAsync(directorId), Times.Once);
    }

    [Fact]
    public async Task DeleteDirectorAsync_NonExistingDirectorId_DoesNotDeleteDirector()
    {
        // Arrange
        var directorId = Guid.NewGuid();

        var directorRepositoryMock = new Mock<IDirectorRepository>();
        var directorService = new DirectorService(directorRepositoryMock.Object);

        directorRepositoryMock
            .Setup(repo => repo.DeleteDirectorAsync(directorId))
            .ReturnsAsync(false);

        // Act and Assert
        await Assert.ThrowsAsync<MultiValidationException>(() => directorService.DeleteDirectorAsync(directorId));
    }

    [Fact]
    public async Task DeleteDirectorAsync_FailedToDelete_ThrowsMultiValidationException()
    {
        // Arrange
        var directorId = Guid.NewGuid();

        var directorRepositoryMock = new Mock<IDirectorRepository>();
        var directorService = new DirectorService(directorRepositoryMock.Object);

        directorRepositoryMock
            .Setup(repo => repo.GetByIdAsync(directorId))
            .ReturnsAsync(new Director { Id = directorId });

        directorRepositoryMock
            .Setup(repo => repo.DeleteDirectorAsync(directorId))
            .ReturnsAsync(false);

        // Act and Assert
        await Assert.ThrowsAsync<MultiValidationException>(async () =>
        {
            await directorService.DeleteDirectorAsync(directorId);
        });
    }

    [Fact]
    public async Task GetAllDirectorsAsync_NoFilter_ReturnsAllDirectors()
    {
        // Arrange
        var expectedDirectors = new List<Director>
        {
            new Director(Guid.NewGuid(), "Director1"),
            new Director(Guid.NewGuid(), "Director2"),
            new Director(Guid.NewGuid(), "Director3")
        };

        var directorRepositoryMock = new Mock<IDirectorRepository>();
        var directorService = new DirectorService(directorRepositoryMock.Object);

        directorRepositoryMock
            .Setup(repo => repo.GetAllDirectorsAsync())
            .ReturnsAsync(expectedDirectors);

        // Act
        var result = await directorService.GetAllDirectorsAsync();

        // Assert
        result.Should().NotBeNull().And.BeAssignableTo<List<DirectorResponse>>();
    }

    [Fact]
    public async Task GetAllDirectorsAsync_WithFilter_ReturnsFilteredDirectors()
    {
        // Arrange
        var expectedDirectors = new List<Director>
        {
            new Director(Guid.NewGuid(), "Director1"),
            new Director(Guid.NewGuid(), "Director2"),
            new Director(Guid.NewGuid(), "Director3")
        };

        var filterByName = "2";

        var directorRepositoryMock = new Mock<IDirectorRepository>();
        var directorService = new DirectorService(directorRepositoryMock.Object);

        directorRepositoryMock
            .Setup(repo => repo.GetAllDirectorsAsync())
            .ReturnsAsync(expectedDirectors);

        // Act
        var result = await directorService.GetAllDirectorsAsync(filterByName);

        // Assert
        result.Should().NotBeNull().And.BeAssignableTo<List<DirectorResponse>>();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdateDirectorAsync_ExistingDirector_ReturnsUpdatedDirectorResponse()
    {
        // Arrange
        var directorId = Guid.NewGuid();
        var directorDto = new DirectorDto { DirectorName = "UpdatedDirector" };

        var expectedDirector = new Director(directorId, "OriginalDirector");

        var directorRepositoryMock = new Mock<IDirectorRepository>();
        var directorService = new DirectorService(directorRepositoryMock.Object);

        directorRepositoryMock
            .Setup(repo => repo.GetByIdAsync(directorId))
            .ReturnsAsync(expectedDirector);

        directorRepositoryMock
            .Setup(repo => repo.UpdateDirectorAsync(directorId, It.IsAny<Director>()))
            .ReturnsAsync(new Director(directorId, "UpdatedDirector"));

        // Act
        var result = await directorService.UpdateDirectorAsync(directorId, directorDto);

        // Assert
        result.Should().NotBeNull().And.BeOfType<DirectorResponse>();
        result.DirectorId.Should().Be(expectedDirector.Id);
        result.DirectorName.Should().Be("UpdatedDirector");
    }

    [Fact]
    public async Task UpdateDirectorAsync_NonExistingDirector_ThrowsMultiValidationException()
    {
        // Arrange
        var directorId = Guid.NewGuid();
        var directorDto = new DirectorDto { DirectorName = "UpdatedDirector" };

        var directorRepositoryMock = new Mock<IDirectorRepository>();
        var directorService = new DirectorService(directorRepositoryMock.Object);

        directorRepositoryMock
            .Setup(repo => repo.GetByIdAsync(directorId))
            .ReturnsAsync((Director)null!);

        // Act and Assert
        await Assert.ThrowsAsync<MultiValidationException>(async () =>
        {
            await directorService.UpdateDirectorAsync(directorId, directorDto);
        });
    }

    [Fact]
    public async Task UpdateDirectorAsync_EmptyDirectorName_ThrowsMultiValidationException()
    {
        // Arrange
        var directorId = Guid.NewGuid();
        var directorDto = new DirectorDto { DirectorName = "" };

        var directorRepositoryMock = new Mock<IDirectorRepository>();
        var directorService = new DirectorService(directorRepositoryMock.Object);

        // Act and Assert
        await Assert.ThrowsAsync<MultiValidationException>(async () =>
        {
            await directorService.UpdateDirectorAsync(directorId, directorDto);
        });
    }

    [Fact]
    public async Task UpdateDirectorAsync_UpdateFailed_ThrowsMultiValidationException()
    {
        // Arrange
        var directorId = Guid.NewGuid();
        var directorDto = new DirectorDto { DirectorName = "UpdatedDirector" };

        var expectedDirector = new Director(directorId, "OriginalDirector");

        var directorRepositoryMock = new Mock<IDirectorRepository>();
        var directorService = new DirectorService(directorRepositoryMock.Object);

        directorRepositoryMock
            .Setup(repo => repo.GetByIdAsync(directorId))
            .ReturnsAsync(expectedDirector);

        directorRepositoryMock
            .Setup(repo => repo.UpdateDirectorAsync(directorId, It.IsAny<Director>()))
            .ReturnsAsync((Director)null!);

        // Act and Assert
        await Assert.ThrowsAsync<MultiValidationException>(async () =>
        {
            await directorService.UpdateDirectorAsync(directorId, directorDto);
        });
    }
}