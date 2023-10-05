using System.Net;
using CinemaApplication.Middleware;
using ClassLibrary1.BodyRequest;
using ClassLibrary1.Dtos;
using ClassLibrary1.ServiceInterface;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApplication.Controllers;
[ApiController]
[Route("[controller]")]
public class MovieController: ControllerBase
{
    private readonly IMovieService _movieService;

    public MovieController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpPost]
    public async Task<IActionResult> AddMovie(MovieDto movieDto)
    {
        try
        {
            var result = await _movieService.CreateMovieAsync(movieDto);
            return Ok(result);
        }
        catch (MultiValidationException multiValidationException)
        {
            return UnprocessableEntity(new ExceptionResponse
            {
                StatusCode = (int)HttpStatusCode.UnprocessableEntity,
                Message = "Validation errors occurred.",
                ValidationErrors = multiValidationException.ValidationErrors
            });
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> GetMovieById(Guid movieId)
    {
        var movieResponse = await _movieService.GetByIdAsync(movieId);
        return Ok(movieResponse);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteMovie(Guid movieId)
    {
        await _movieService.DeleteAsync(movieId);
        return Ok(new { message = "Movie deleted successfully." });
    }
    
    [HttpGet("movies")]
    public async Task<IActionResult> GetAllMoviesAsync([FromQuery] string? filterByName = null)
    {
        var movieResponses = await _movieService.GetAllMoviesAsync(filterByName);

            if (movieResponses.Count == 0)
            {
                return NotFound("No movies found matching the filter.");
            }

            return Ok(movieResponses);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateMovie(Guid movieId, [FromBody] MovieBodyRequest movieRequest)
    {
        var result = await _movieService.UpdateMovieAsync(movieId, movieRequest);
        return Ok(result);
    }
    
    [HttpDelete("Delete movies")]
    public async Task<IActionResult> DeleteMovies(List<string> movieIds)
    {
        var response= await _movieService.DeleteMoviesAsync(movieIds);
        return Ok(response);
    }
}