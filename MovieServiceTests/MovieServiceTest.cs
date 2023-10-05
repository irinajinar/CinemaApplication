using ClassLibrary1.BodyRequest;
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

public class MovieServiceTest
{
    [Fact]
    public async Task CreateMovieAsync_ReturnsMovieResponse()
    {
        // Arrange
        var movieDto = new MovieDto
        {
            Name = "Hulk",
            Description = "Hulk description"
        };
        var movieRepositoryMock = new Mock<IMovieRepository>();
        var movieService = new MovieService(movieRepositoryMock.Object);
       
        // Act
        var result = await movieService.CreateMovieAsync(movieDto);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<MovieResponse>();
    }
    
    [Fact]
    public async Task CreateMovieAsync_ThrowsMultiValidationException_WhenValidationFails()
    {
        // Arrange
        var movieDto = new MovieDto
        {
            Name = "", 
            Description = ""
        };

        var movieRepositoryMock = new Mock<IMovieRepository>();
        var movieService = new MovieService(movieRepositoryMock.Object);

        // Act and Assert
        Func<Task> act = async () => await movieService.CreateMovieAsync(movieDto);
        var exceptionForName = await act.Should().ThrowAsync<MultiValidationException>();
        exceptionForName.Which.ValidationErrors.Should().Contain("Name is required.");
        var exceptionForDescription = await act.Should().ThrowAsync<MultiValidationException>();
        exceptionForDescription.Which.ValidationErrors.Should().Contain("Description is required.");
    }
    
    [Fact]
    public async Task GetByIdAsync_ReturnsMovieResponse_WhenMovieExists()
    {
        // Arrange
        var movieId = Guid.NewGuid();
        var movie = new Movie(movieId, "Test Movie", "Test Description");

        var movieRepositoryMock = new Mock<IMovieRepository>();
        movieRepositoryMock.Setup(repo => repo.GetByIdAsync(movieId)).ReturnsAsync(movie);

        var movieService = new MovieService(movieRepositoryMock.Object);

        // Act
        var result = await movieService.GetByIdAsync(movieId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<MovieResponse>();
        result.MovieId.Should().Be(movieId);
    }
    
    [Fact]
    public async Task GetByIdAsync_ThrowsMultiValidationException_WhenMovieDoesNotExist()
    {
        // Arrange
        var movieId = Guid.NewGuid();

        var movieRepositoryMock = new Mock<IMovieRepository>();
        movieRepositoryMock.Setup(repo => repo.GetByIdAsync(movieId)).ReturnsAsync((Movie)null!);

        var movieService = new MovieService(movieRepositoryMock.Object);

        // Act and Assert
        Func<Task> act = async () => await movieService.GetByIdAsync(movieId);
        await act.Should().ThrowAsync<MultiValidationException>().WithMessage($"The movie with the {movieId} not found");
    }
    
    [Fact]
    public async Task GetAllMoviesAsync_ReturnsAllMovies_WhenFilterByNameIsNull()
    {
        // Arrange
        var movieRepositoryMock = new Mock<IMovieRepository>();
        var movies = new List<Movie>
        {
            new Movie(Guid.NewGuid(), "Movie 1", "Description 1"),
            new Movie(Guid.NewGuid(), "Movie 2", "Description 2"),
            new Movie(Guid.NewGuid(), "Movie 3", "Description 3")
        };
        movieRepositoryMock.Setup(repo => repo.GetAllMoviesAsync()).ReturnsAsync(movies);

        var movieService = new MovieService(movieRepositoryMock.Object);

        // Act
        var result = await movieService.GetAllMoviesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllMoviesAsync_ReturnsFilteredMovies_WhenFilterByNameIsProvided()
    {
        // Arrange
        var movieRepositoryMock = new Mock<IMovieRepository>();
        var movies = new List<Movie>
        {
            new Movie(Guid.NewGuid(), "Movie 1", "Description 1"),
            new Movie(Guid.NewGuid(), "Movie 2", "Description 2"),
            new Movie(Guid.NewGuid(), "Movie 3", "Description 3")
        };
        movieRepositoryMock.Setup(repo => repo.GetAllMoviesAsync()).ReturnsAsync(movies);

        var movieService = new MovieService(movieRepositoryMock.Object);

        // Act
        var result = await movieService.GetAllMoviesAsync("Movie 2");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1); // Ensure only one movie matches the filter
        result.First().MovieName.Should().Be("Movie 2"); // Check the filtered movie's name
    }
    [Fact]
    public async Task DeleteAsync_DeletesMovie_WhenMovieExists()
    {
        // Arrange
        var movieId = Guid.NewGuid();
        var movie = new Movie(movieId, "Test Movie", "Test Description");

        var movieRepositoryMock = new Mock<IMovieRepository>();
        movieRepositoryMock.Setup(repo => repo.GetByIdAsync(movieId)).ReturnsAsync(movie);
        movieRepositoryMock.Setup(repo => repo.DeleteMovieAsync(movieId)).ReturnsAsync(true);

        var movieService = new MovieService(movieRepositoryMock.Object);

        // Act
        var act = async () => await movieService.DeleteAsync(movieId);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DeleteAsync_DoesNothing_WhenMovieDoesNotExist()
    {
        // Arrange
        var movieId = Guid.NewGuid();

        var movieRepositoryMock = new Mock<IMovieRepository>();
        movieRepositoryMock.Setup(repo => repo.GetByIdAsync(movieId))!.ReturnsAsync((Movie)null!);

        var movieService = new MovieService(movieRepositoryMock.Object);

        // Act
        var act = async () => await movieService.DeleteAsync(movieId);

        // Assert
        await act.Should().NotThrowAsync(); // Ensure no exception is thrown
    }
    
    [Fact]
    public async Task DeleteMoviesAsync_DoesNothing_WhenNoMoviesExist()
    {
        // Arrange
        var movieIds = new List<string> { "1", "2", "3" };
        var movieRepositoryMock = new Mock<IMovieRepository>();
        movieRepositoryMock.Setup(repo => repo.DeleteMoviesAsync(movieIds)).ReturnsAsync(false);

        var movieService = new MovieService(movieRepositoryMock.Object);

        // Act
        var result = await movieService.DeleteMoviesAsync(movieIds);

        // Assert
        result.Should().NotBeNull();
        result.Message.Should().Be("No movies were deleted.");
    }
    [Fact]
    public async Task UpdateMovieAsync_UpdatesMovie_WhenMovieExists()
    {
        // Arrange
        var movieId = Guid.NewGuid();
        var movieBodyRequest = new MovieBodyRequest
        {
            Name = "Updated Movie Name",
            Description = "Updated Movie Description"
        };
        var existingMovie = new Movie(movieId, "Original Name", "Original Description");

        var movieRepositoryMock = new Mock<IMovieRepository>();
        movieRepositoryMock.Setup(repo => repo.GetByIdAsync(movieId)).ReturnsAsync(existingMovie);
        movieRepositoryMock.Setup(repo => repo.UpdateMovieAsync(movieId, It.IsAny<Movie>()))
            .ReturnsAsync(existingMovie); // Simulate successful update

        var movieService = new MovieService(movieRepositoryMock.Object);

        // Act
        var result = await movieService.UpdateMovieAsync(movieId, movieBodyRequest);

        // Assert
        result.Should().NotBeNull();
        result.MovieName.Should().Be("Updated Movie Name"); // Check the updated name
        result.MovieDescription.Should().Be("Updated Movie Description"); // Check the updated description
    }

    [Fact]
    public async Task UpdateMovieAsync_ThrowsMultiValidationException_WhenMovieDoesNotExist()
    {
        // Arrange
        var movieId = Guid.NewGuid();
        var movieBodyRequest = new MovieBodyRequest
        {
            Name = "Updated Movie Name",
            Description = "Updated Movie Description"
        };

        var movieRepositoryMock = new Mock<IMovieRepository>();
        movieRepositoryMock.Setup(repo => repo.GetByIdAsync(movieId))!.ReturnsAsync((Movie)null!);

        var movieService = new MovieService(movieRepositoryMock.Object);

        // Act and Assert
        Func<Task> act = async () => await movieService.UpdateMovieAsync(movieId, movieBodyRequest);
        await act.Should().ThrowAsync<MultiValidationException>()
            .WithMessage("Movie not found");
    }

    [Fact]
    public async Task UpdateMovieAsync_ThrowsMultiValidationException_WhenUpdateFails()
    {
        // Arrange
        var movieId = Guid.NewGuid();
        var movieBodyRequest = new MovieBodyRequest
        {
            Name = "Updated Movie Name",
            Description = "Updated Movie Description"
        };
        var existingMovie = new Movie(movieId, "Original Name", "Original Description");

        var movieRepositoryMock = new Mock<IMovieRepository>();
        movieRepositoryMock.Setup(repo => repo.GetByIdAsync(movieId)).ReturnsAsync(existingMovie);
        movieRepositoryMock.Setup(repo => repo.UpdateMovieAsync(movieId, It.IsAny<Movie>()))!
            .ReturnsAsync((Movie)null!); // Simulate update failure

        var movieService = new MovieService(movieRepositoryMock.Object);

        // Act and Assert
        Func<Task> act = async () => await movieService.UpdateMovieAsync(movieId, movieBodyRequest);
        await act.Should().ThrowAsync<MultiValidationException>()
            .WithMessage("Failed to update the movie.");
    }
}
