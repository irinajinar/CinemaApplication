using ClassLibrary1.BodyRequest;
using ClassLibrary1.Dtos;
using ClassLibrary1.Responses;

namespace ClassLibrary1.ServiceInterface;

public interface IActorService
{
    Task<ActorResponse> AddActorAsync(ActorDto actorDto);
    Task<List<ActorResponse>> GetAllActorsAsync(string? filterByName = null, Guid? filterByMovieId = null);
    Task DeleteAsync(Guid actorId);
    Task<ActorResponse> GetByIdAsync(Guid actorId);
    Task<ActorResponse> UpdateActorAsync(Guid actorId, ActorBodyRequest actorBodyRequest);
}