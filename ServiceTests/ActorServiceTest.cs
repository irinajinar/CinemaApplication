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

public class ActorServiceTest
{
    [Fact]
    public async Task AddActorAsync_ShouldReturnValidActorResponse_WhenActorIsValid()
    {
        // Arrange
        var actorDto = new ActorDto
        {
            Name = "John Doe",
            Age = 35,
            Country = "USA",
            Biography = "A talented actor",
            MovieId = Guid.NewGuid()
        };

        var actorRepositoryMock = new Mock<IActorRepository>();
        var movieRepositoryMock = new Mock<IMovieRepository>();

        // Mock the behavior of the movie repository
        movieRepositoryMock.Setup(repo => repo.GetByIdAsync(actorDto.MovieId))
            .ReturnsAsync(new Movie(actorDto.MovieId, "Test Movie", "Test Description", 2023));

        var actorService = new ActorService(actorRepositoryMock.Object, movieRepositoryMock.Object);

        // Act
        var result = await actorService.AddActorAsync(actorDto);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ActorResponse>();
    }

    [Fact]
    public async Task AddActorAsync_ShouldThrowMultiValidationException_WhenActorIsNotValid()
    {
        // Arrange
        var actorDto = new ActorDto
        {
            Name = "",
            Age = -5,
            Country = null!,
            Biography = "",
            MovieId = Guid.Empty
        };

        var actorRepositoryMock = new Mock<IActorRepository>();
        var movieRepositoryMock = new Mock<IMovieRepository>();
        var actorService = new ActorService(actorRepositoryMock.Object, movieRepositoryMock.Object);

        // Act and Assert
        await Assert.ThrowsAsync<MultiValidationException>(async () => await actorService.AddActorAsync(actorDto));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsActorResponse_WhenActorExists()
    {
        // Arrange
        var existingActorId = Guid.NewGuid();
        var actor = new Actor(existingActorId, "Test", 30, "USA", "TestBio");

        var actorRepositoryMock = new Mock<IActorRepository>();
        var movieRepositoryMock = new Mock<IMovieRepository>();

        actorRepositoryMock.Setup(repo => repo.GetActorWithMoviesAsync(existingActorId)).ReturnsAsync(actor);

        var actorService = new ActorService(actorRepositoryMock.Object, movieRepositoryMock.Object);

        // Act
        var result = await actorService.GetByIdAsync(existingActorId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ActorResponse>();
        result.Id.Should().Be(existingActorId);
        result.Name.Should().Be("Test");
        result.Age.Should().Be(30);
        result.Country.Should().Be("USA");
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsMultiValidationException_WhenActorDoesNotExists()
    {
        // Arrange
        var actorId = Guid.NewGuid();
        var actorRepositoryMock = new Mock<IActorRepository>();
        var movieRepositoryMock = new Mock<IMovieRepository>();

        actorRepositoryMock.Setup(repo =>
            repo.GetActorWithMoviesAsync(actorId)).ReturnsAsync((Actor)null!);

        var actorService = new ActorService(actorRepositoryMock.Object, movieRepositoryMock.Object);

        // Act and Assert
        Func<Task> act = async () => await actorService.GetByIdAsync(actorId);
        await act.Should().ThrowAsync<MultiValidationException>()
            .WithMessage($"The actor with the ID {actorId} not found");
    }

    [Fact]
    public async Task GetAllActorsAsync_ReturnsAllActors_WhenFilterByNameIsNull()
    {
        //Arrange
        var actorRepositoryMock = new Mock<IActorRepository>();
        var movieRepositoryMock = new Mock<IMovieRepository>();
        var actors = new List<Actor>
        {
            new Actor(Guid.NewGuid(), "Actor1", 20, "Spain", "Bio"),
            new Actor(Guid.NewGuid(), "Actor2", 30, "Italy", "Bio"),
            new Actor(Guid.NewGuid(), "Actor3", 40, "France", "Bio")
        };
        actorRepositoryMock.Setup(repo => repo.GetAllActorsAsync())!.ReturnsAsync(actors);
        var actorService = new ActorService(actorRepositoryMock.Object, movieRepositoryMock.Object);
        //Act
        var result = await actorService.GetAllActorsAsync();
        //Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllActorsAsync_ReturnsFilteredActors_WhenFilterIsProvided()
    {
        //Arrange
        var actorRepositoryMock = new Mock<IActorRepository>();
        var movieRepositoryMock = new Mock<IMovieRepository>();
        var actors = new List<Actor>
        {
            new Actor(Guid.NewGuid(), "Actor1", 20, "Spain", "Bio"),
            new Actor(Guid.NewGuid(), "Actor2", 30, "Italy", "Bio"),
            new Actor(Guid.NewGuid(), "Actor3", 40, "France", "Bio")
        };
        actorRepositoryMock.Setup(repo => repo.GetAllActorsAsync())!.ReturnsAsync(actors);
        var actorService = new ActorService(actorRepositoryMock.Object, movieRepositoryMock.Object);
        //Act
        var result = actorService.GetAllActorsAsync("Actor1");
        //Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_DeleteActor_WhenActorExists()
    {
        // Arrange
        var actorId = Guid.NewGuid();

        var actorRepositoryMock = new Mock<IActorRepository>();
        var movieRepositoryMock = new Mock<IMovieRepository>();

        actorRepositoryMock.Setup(repo => repo.DeleteActorAsync(actorId)).ReturnsAsync(true);

        var actorService = new ActorService(actorRepositoryMock.Object, movieRepositoryMock.Object);

        // Act
        var act = async () => await actorService.DeleteAsync(actorId);
        //Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DeleteAsync_DoesNothing_WhenActorDoesNotExists()
    {
        //Arrange
        var actorId = Guid.NewGuid();
        var actorRepositoryMock = new Mock<IActorRepository>();
        var movieRepositoryMock = new Mock<IMovieRepository>();

        actorRepositoryMock.Setup(repo => repo.DeleteActorAsync(actorId)).ReturnsAsync(true);

        var actorService = new ActorService(actorRepositoryMock.Object, movieRepositoryMock.Object);

        //Act
        var act = async () => await actorService.DeleteAsync(actorId);
        //Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task UpdateActorAsync_UpdateActor_WhenActorExists()
    {
        // Arrange
        var existingActorId = Guid.NewGuid();
        var actorBodyRequest = new ActorBodyRequest("Actor11", 20, "SUA", "Bio", existingActorId);

        var existingActor = new Actor(existingActorId, "OriginalName", 30, "OriginalCountry", "OriginalBio");

        var actorRepositoryMock = new Mock<IActorRepository>();
        var movieRepositoryMock = new Mock<IMovieRepository>();

        actorRepositoryMock.Setup(repo => repo.GetByIdAsync(existingActorId)).ReturnsAsync(existingActor);

        var actorService = new ActorService(actorRepositoryMock.Object, movieRepositoryMock.Object);

        // Act
        var result = await actorService.UpdateActorAsync(existingActorId, actorBodyRequest);

        // Assert
        result.Should().NotBeNull();
        // Add more specific assertions if needed
        result.Name.Should().Be("Actor11");
        result.Age.Should().Be(20);
        result.Biography.Should().Be("Bio");
        result.Country.Should().Be("SUA");
    }
}