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
        var directorDto = new DirectorDto
        {
            DirectorName = "Smith"
        };
        var directorId = Guid.NewGuid();
        var movieDto = new MovieWithActorsRequest()
        {
            Name = "Hulk",
            Description = "Hulk description",
            Year = 2010,
            DirectorId = directorId,
            Actors = new List<ActorBodyRequest>
            {
                new ActorBodyRequest("Actor1", 30, "Country1", "Bio1", Guid.NewGuid())
            }
        };
        var movieRepositoryMock = new Mock<IMovieRepository>();
        var directorRepositoryMock = new Mock<IDirectorRepository>();
        var actorRepositoryMock = new Mock<IActorRepository>();
        var movieService = new MovieService(movieRepositoryMock.Object, directorRepositoryMock.Object,
            actorRepositoryMock.Object);

        var expectedDirector = new Director
        {
            Id = directorId,
            Name = directorDto.DirectorName
        };

        directorRepositoryMock
            .Setup(repo => repo.GetByIdAsync(directorId))
            .ReturnsAsync(expectedDirector);
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
        var movieDto = new MovieWithActorsRequest()
        {
            Name = "",
            Description = ""
        };

        var movieRepositoryMock = new Mock<IMovieRepository>();
        var directorRepositoryMock = new Mock<IDirectorRepository>();
        var actorRepositoryMock = new Mock<IActorRepository>();
        var movieService = new MovieService(movieRepositoryMock.Object, directorRepositoryMock.Object,
            actorRepositoryMock.Object);

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
        var movie = new Movie(movieId, "Test Movie", "Test Description", 1234);

        var movieRepositoryMock = new Mock<IMovieRepository>();
        var directorRepositoryMock = new Mock<IDirectorRepository>();
        var actorRepositoryMock = new Mock<IActorRepository>();
        movieRepositoryMock.Setup(repo => repo.GetMovieWithActorsAsync(movieId)).ReturnsAsync(movie);

        var movieService = new MovieService(movieRepositoryMock.Object, directorRepositoryMock.Object,
            actorRepositoryMock.Object);

        // Act
        var result = await movieService.GetByIdAsync(movieId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<MovieResponse>();
        result.MovieId.Should().Be(movieId);
        result.MovieName.Should().Be("Test Movie");
        result.MovieDescription.Should().Be("Test Description");
        result.YearOfProduction.Should().Be(1234);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsMultiValidationException_WhenMovieDoesNotExist()
    {
        // Arrange
        var movieId = Guid.NewGuid();

        var movieRepositoryMock = new Mock<IMovieRepository>();
        var directorRepositoryMock = new Mock<IDirectorRepository>();
        var actorRepositoryMock = new Mock<IActorRepository>();
        movieRepositoryMock.Setup(repo =>
            repo.GetByIdAsync(movieId)).ReturnsAsync((Movie)null!);

        var movieService = new MovieService(movieRepositoryMock.Object, directorRepositoryMock.Object,
            actorRepositoryMock.Object);

        // Act and Assert
        Func<Task> act = async () => await movieService.GetByIdAsync(movieId);
        await act.Should().ThrowAsync<MultiValidationException>()
            .WithMessage($"The movie with the {movieId} not found");
    }

    [Fact]
    public async Task GetAllMoviesAsync_ReturnsAllMovies_WhenFilterByNameIsNull()
    {
        // Arrange
        var movieRepositoryMock = new Mock<IMovieRepository>();
        var directorRepositoryMock = new Mock<IDirectorRepository>();
        var actorRepositoryMock = new Mock<IActorRepository>();
        var movies = new List<Movie>
        {
            new Movie(Guid.NewGuid(), "Movie 1", "Description 1", 1990),
            new Movie(Guid.NewGuid(), "Movie 2", "Description 2", 1991),
            new Movie(Guid.NewGuid(), "Movie 3", "Description 3", 1993)
        };
        movieRepositoryMock.Setup(repo => repo.GetAllMoviesAsync())!.ReturnsAsync(movies);

        var movieService = new MovieService(movieRepositoryMock.Object, directorRepositoryMock.Object,
            actorRepositoryMock.Object);

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
        var directorRepositoryMock = new Mock<IDirectorRepository>();
        var actorRepositoryMock = new Mock<IActorRepository>();
        var movies = new List<Movie>
        {
            new Movie(Guid.NewGuid(), "Movie 1", "Description 1", 1990),
            new Movie(Guid.NewGuid(), "Movie 2", "Description 2", 1991),
            new Movie(Guid.NewGuid(), "Movie 3", "Description 3", 1992)
        };
        movieRepositoryMock.Setup(repo => repo.GetAllMoviesAsync())!.ReturnsAsync(movies);

        var movieService = new MovieService(movieRepositoryMock.Object, directorRepositoryMock.Object,
            actorRepositoryMock.Object);

        // Act
        var result = await movieService.GetAllMoviesAsync("Movie 2");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().MovieName.Should().Be("Movie 2");
    }

    [Fact]
    public async Task DeleteAsync_DeletesMovie_WhenMovieExists()
    {
        // Arrange
        var movieId = Guid.NewGuid();
        var movie = new Movie(movieId, "Test Movie", "Test Description", 1900);

        var movieRepositoryMock = new Mock<IMovieRepository>();
        var directorRepositoryMock = new Mock<IDirectorRepository>();
        var actorRepositoryMock = new Mock<IActorRepository>();
        movieRepositoryMock.Setup(repo => repo.GetByIdAsync(movieId)).ReturnsAsync(movie);
        movieRepositoryMock.Setup(repo => repo.DeleteMovieAsync(movieId)).ReturnsAsync(true);

        var movieService = new MovieService(movieRepositoryMock.Object, directorRepositoryMock.Object,
            actorRepositoryMock.Object);

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
        var directorRepositoryMock = new Mock<IDirectorRepository>();
        var actorRepositoryMock = new Mock<IActorRepository>();

        movieRepositoryMock.Setup(repo => repo.GetByIdAsync(movieId)).ReturnsAsync((Movie)null!);

        var movieService = new MovieService(movieRepositoryMock.Object, directorRepositoryMock.Object,
            actorRepositoryMock.Object);

        // Act and Assert
        var act = async () => await movieService.DeleteAsync(movieId);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DeleteMoviesAsync_DoesNothing_WhenNoMoviesExist()
    {
        // Arrange
        var movieIds = new List<string> { "1", "2", "3" };
        var movieRepositoryMock = new Mock<IMovieRepository>();
        var directorRepositoryMock = new Mock<IDirectorRepository>();
        var actorRepositoryMock = new Mock<IActorRepository>();
        movieRepositoryMock.Setup(repo => repo.DeleteMoviesAsync(movieIds)).ReturnsAsync(false);

        var movieService = new MovieService(movieRepositoryMock.Object, directorRepositoryMock.Object,
            actorRepositoryMock.Object);

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
            Description = "Updated Movie Description",
            Year = 2003,
            DirectorId = Guid.NewGuid(),
            ActorsToAdd = new List<ActorBodyRequest>
            {
                new ActorBodyRequest("Actor1", 30, "Country1", "Bio1", Guid.NewGuid())
            },
            ActorsToRemove = new List<Guid>()
        };

        var existingMovie = new Movie(movieId, "Original Name", "Original Description", 1995);

        var movieRepositoryMock = new Mock<IMovieRepository>();
        var directorRepositoryMock = new Mock<IDirectorRepository>();
        var actorRepositoryMock = new Mock<IActorRepository>();

        movieRepositoryMock.Setup(repo => repo.GetMovieWithActorsAsync(movieId)).ReturnsAsync(existingMovie);
        movieRepositoryMock.Setup(repo => repo.UpdateMovieAsync(movieId, It.IsAny<Movie>()))
            .ReturnsAsync(existingMovie);


        var movieService = new MovieService(movieRepositoryMock.Object, directorRepositoryMock.Object,
            actorRepositoryMock.Object);

        // Act
        var result = await movieService.UpdateMovieAsync(movieId, movieBodyRequest);

        // Assert
        result.Should().NotBeNull();
        result.MovieName.Should().Be("Updated Movie Name");
        result.MovieDescription.Should().Be("Updated Movie Description");
        result.YearOfProduction.Should().Be(2003);
        result.DirectorId.Should().Be(movieBodyRequest.DirectorId);
    }
}