using System.Net;
using CinemaApplication.Middleware;
using ClassLibrary1.Dtos;
using ClassLibrary1.ServiceInterface;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;


namespace CinemaApplication.Controllers
{
    [ApiController]
    [Route("api/director")]
    public class DirectorController : ControllerBase
    {
        private readonly IDirectorService _directorService;

        public DirectorController(IDirectorService directorService)
        {
            _directorService = directorService;
        }

        [HttpPost]
        public async Task<IActionResult> AddDirectorAsync(DirectorDto directorDto)
        {
            try
            {
                var result = await _directorService.AddDirectorAsync(directorDto);
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
        public async Task<IActionResult> GetDirectorByIdAsync(Guid directorId)
        {
            var directorResponse = await _directorService.GetByIdAsync(directorId);
            return Ok(directorResponse);
        }

        [HttpGet("directors")]
        public async Task<IActionResult> GetAllDirectorsAsync([FromQuery] string? filterByName = null)
        {
            var directorResponses = await _directorService.GetAllDirectorsAsync(filterByName);

            if (directorResponses.Count == 0)
            {
                return NotFound("No movies found matching the filter.");
            }

            return Ok(directorResponses);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDirectorAsync(Guid movieId)
        {
            await _directorService.DeleteDirectorAsync(movieId);
            return Ok(new { message = "Director deleted successfully." });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDirectorAsync(Guid directorId, [FromBody] DirectorDto directorDto)
        {
            var result = await _directorService.UpdateDirectorAsync(directorId, directorDto);
            return Ok(result);
        }
    }
}