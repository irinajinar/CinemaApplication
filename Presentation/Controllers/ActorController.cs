using System.Net;
using CinemaApplication.Middleware;
using ClassLibrary1.BodyRequest;
using ClassLibrary1.Dtos;
using ClassLibrary1.ServiceInterface;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApplication.Controllers;

[ApiController]
[Route("api/actors")]
public class ActorController : ControllerBase
{
    private readonly IActorService _actorService;

    public ActorController(IActorService actorService)
    {
        _actorService = actorService;
    }

    [HttpPost]
    public async Task<IActionResult> AddActorAsync(ActorDto actorDto)
    {
        try
        {
            var result = await _actorService.AddActorAsync(actorDto);
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
    public async Task<IActionResult> GetActorByIdAsync(Guid actorId)
    {
        var actorResponse = await _actorService.GetByIdAsync(actorId);
        return Ok(actorResponse);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteActorAsync(Guid actorId)
    {
        await _actorService.DeleteAsync(actorId);
        return Ok(new { message = "Actor deleted successfully." });
    }

    [HttpGet("actors")]
    public async Task<IActionResult> GetAllActorsAsync([FromQuery] string? filterByName = null,
        Guid? filterByMovieId = null)
    {
        var actorsResponses = await _actorService.GetAllActorsAsync(filterByName, filterByMovieId);

        if (actorsResponses.Count == 0)
        {
            return NotFound("No actors found matching the filters.");
        }

        return Ok(actorsResponses);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateActorAsync(Guid actorId, [FromBody] ActorBodyRequest actorBodyRequest)
    {
        var result = await _actorService.UpdateActorAsync(actorId, actorBodyRequest);
        return Ok(result);
    }
}