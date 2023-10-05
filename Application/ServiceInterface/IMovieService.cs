using ClassLibrary1.BodyRequest;
using ClassLibrary1.Dtos;
using ClassLibrary1.Responses;

namespace ClassLibrary1.ServiceInterface;

public interface IMovieService
{
    Task<MovieResponse> CreateMovieAsync(MovieDto movieDto);
    Task<MovieResponse> GetByIdAsync(Guid movieId);
    Task DeleteAsync(Guid movieId);
    Task<List<MovieResponse>> GetAllMoviesAsync(string? filterByName = null);
    Task<MovieResponse> UpdateMovieAsync(Guid movieId, MovieBodyRequest movieBodyRequest);
    Task<DeleteManyResponse> DeleteMoviesAsync(List<string> movieIds);
}