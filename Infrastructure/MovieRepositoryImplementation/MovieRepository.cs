using ClassLibrary1.RepositoryInterfaces;
using Domain.Exceptions;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.MovieRepositoryImplementation;

public class MovieRepository: IMovieRepository
{
    private readonly DataContext.DataAppContext _dataAppContext;

    public MovieRepository(DataContext.DataAppContext dataAppContext)
    {
        _dataAppContext = dataAppContext;
    }
    public async Task AddMovieAsync(Movie movie)
    {
        await _dataAppContext.Movies.AddAsync(movie);
         await _dataAppContext.SaveChangesAsync();
    }

    public async Task<Movie> GetByIdAsync (Guid movieId)
    {
       return await _dataAppContext.Movies.FindAsync(movieId);
    }

    public async Task<bool> DeleteMovieAsync(Guid movieId)
    {
       var movie = await _dataAppContext.Movies.FindAsync(movieId);
       if (movie == null)
       {
           return false;
       }

       _dataAppContext.Movies.Remove(movie);
       await _dataAppContext.SaveChangesAsync();
       return true;
    }

    public async Task<List<Movie>> GetAllMoviesAsync()
    {
        var movies = await _dataAppContext.Movies.ToListAsync();
        return movies;
    }

    public async Task<Movie> UpdateMovieAsync(Guid movieId, Movie updatedMovie)
    {
        var movie = await _dataAppContext.Movies.FindAsync(movieId);
        if (movie == null)
        {
            return null;
        }

        movie.Name = updatedMovie.Name;
        movie.Description = updatedMovie.Description;
        await _dataAppContext.SaveChangesAsync();
        return movie;
    }

    public async Task<bool> DeleteMoviesAsync(List<string> movieIds)
    {
        foreach (var movieId in movieIds)
        {
            if (Guid.TryParse(movieId, out var parsedMovieId))
            {
                var movieToDelete = await _dataAppContext.Movies.FindAsync(parsedMovieId);

                if (movieToDelete != null)
                {
                    _dataAppContext.Movies.Remove(movieToDelete);
                }
            }
        }

        await _dataAppContext.SaveChangesAsync();
        return true;
    }
}