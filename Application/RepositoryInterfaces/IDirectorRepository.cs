using Domain.Models;

namespace ClassLibrary1.RepositoryInterfaces;

public interface IDirectorRepository
{
    Task AddDirectorAsync(Director director);
    Task<Director> GetByIdAsync(Guid directorId);
    Task<bool> DeleteDirectorAsync(Guid directorId);
    Task<List<Director>> GetAllDirectorsAsync();
    Task<Director?> UpdateDirectorAsync(Guid director, Director updateDirector);
}