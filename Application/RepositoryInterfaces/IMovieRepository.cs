using Domain.Models;

namespace ClassLibrary1.RepositoryInterfaces;

public interface IMovieRepository
{
    Task AddMovieAsync(Movie movie);
    Task<Movie> GetByIdAsync(Guid movieId);
    Task<bool> DeleteMovieAsync(Guid movieId);
    Task<List<Movie>> GetAllMoviesAsync();
    Task<Movie> UpdateMovieAsync(Guid movie, Movie updatedMovie);
    Task<bool> DeleteMoviesAsync(List<string> moviesIds);
}