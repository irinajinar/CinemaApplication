using System.Net;
using CinemaApplication.Middleware;
using ClassLibrary1.BodyRequest;
using ClassLibrary1.ServiceInterface;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApplication.Controllers;

[ApiController]
[Route("api/movies")]
public class MovieController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MovieController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpPost]
    public async Task<IActionResult> AddMovieAsync([FromBody] MovieWithActorsRequest movieRequest)
    {
        try
        {
            var result = await _movieService.CreateMovieAsync(movieRequest);
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
    public async Task<IActionResult> GetMovieByIdAsync(Guid movieId)
    {
        var movieResponse = await _movieService.GetByIdAsync(movieId);
        return Ok(movieResponse);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteMovieAsync(Guid movieId)
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
    public async Task<IActionResult> UpdateMovieAsync(Guid movieId, [FromBody] MovieBodyRequest movieRequest)
    {
        var result = await _movieService.UpdateMovieAsync(movieId, movieRequest);
        return Ok(result);
    }

    [HttpDelete("Delete movies")]
    public async Task<IActionResult> DeleteMoviesAsync(List<string> movieIds)
    {
        var response = await _movieService.DeleteMoviesAsync(movieIds);
        return Ok(response);
    }
}