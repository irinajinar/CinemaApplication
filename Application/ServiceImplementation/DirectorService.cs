using ClassLibrary1.Dtos;
using ClassLibrary1.RepositoryInterfaces;
using ClassLibrary1.Responses;
using ClassLibrary1.ServiceInterface;
using Domain.Exceptions;
using Domain.Models;

namespace ClassLibrary1.ServiceImplementation;

public class DirectorService : IDirectorService
{
    private readonly IDirectorRepository _directorRepository;

    public DirectorService(IDirectorRepository directorRepository)
    {
        _directorRepository = directorRepository;
    }

    public async Task<DirectorResponse> AddDirectorAsync(DirectorDto directorDto)
    {
        var validationErrors = new List<string>();
        if (string.IsNullOrWhiteSpace(directorDto.DirectorName))
        {
            validationErrors.Add("Name is required.");
        }

        if (validationErrors.Count > 0)
        {
            throw new MultiValidationException(validationErrors);
        }

        var newDirector = new Director(Guid.NewGuid(), directorDto.DirectorName);
        await _directorRepository.AddDirectorAsync(newDirector);

        return new DirectorResponse(newDirector.Id, newDirector.Name);
    }

    public async Task<DirectorResponse> GetByIdAsync(Guid directorId)
    {
        var director = await _directorRepository.GetByIdAsync(directorId);
        if (director == null)
        {
            throw new MultiValidationException($"The director with the {directorId} not found");
        }

        return new DirectorResponse(
            director.Id,
            director.Name);
    }

    public async Task DeleteDirectorAsync(Guid directorId)
    {
        var isDeleted = await _directorRepository.DeleteDirectorAsync(directorId);
        if (!isDeleted)
        {
            throw new MultiValidationException("Failed to delete the director.");
        }
    }

    public async Task<List<DirectorResponse>> GetAllDirectorsAsync(string? filterByName = null)
    {
        var directors = await _directorRepository.GetAllDirectorsAsync();
        if (!string.IsNullOrEmpty(filterByName))
        {
            directors = directors.Where(d => d.Name.Contains(filterByName, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var directorsResponses = directors.Select(director => new DirectorResponse(
            director.Id,
            director.Name
        )).ToList();
        return directorsResponses;
    }

    public async Task<DirectorResponse> UpdateDirectorAsync(Guid directorId, DirectorDto directorDto)
    {
        var existingDirector = await _directorRepository.GetByIdAsync(directorId);
        if (existingDirector == null)
        {
            throw new MultiValidationException("Director not found");
        }

        if (string.IsNullOrWhiteSpace(directorDto.DirectorName))
        {
            throw new MultiValidationException("Name is required.");
        }

        existingDirector.Name = directorDto.DirectorName;

        var updatedDirector = await _directorRepository.UpdateDirectorAsync(directorId, existingDirector);
        if (updatedDirector == null)
        {
            throw new MultiValidationException("Failed to update the director.");
        }

        return new DirectorResponse(updatedDirector.Id, updatedDirector.Name);
    }
}