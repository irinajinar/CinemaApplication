using ClassLibrary1.Dtos;
using ClassLibrary1.Responses;

namespace ClassLibrary1.ServiceInterface;

public interface IDirectorService
{
    Task<DirectorResponse> AddDirectorAsync(DirectorDto directorDto);
    Task<DirectorResponse> GetByIdAsync(Guid directorId);
    Task DeleteDirectorAsync(Guid directorId);
    Task<List<DirectorResponse>> GetAllDirectorsAsync(string? filterByName = null);
    Task<DirectorResponse> UpdateDirectorAsync(Guid directorId, DirectorDto directorDto);
}