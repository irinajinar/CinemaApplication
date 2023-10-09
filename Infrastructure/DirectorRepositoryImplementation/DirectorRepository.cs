using ClassLibrary1.RepositoryInterfaces;
using Domain.Models;
using Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DirectorRepositoryImplementation;

public class DirectorRepository:IDirectorRepository
{
    private readonly DataAppContext _dataAppContext;

    public DirectorRepository(DataAppContext dataAppContext)
    {
        _dataAppContext = dataAppContext;
    }

    public async Task AddDirectorAsync(Director director)
    {
        await _dataAppContext.Directors.AddAsync(director);
        await _dataAppContext.SaveChangesAsync();
    }

    public async Task<Director> GetByIdAsync(Guid directorId)
    {
       return (await _dataAppContext.Directors.FindAsync(directorId))!;
    }

    public async Task<bool> DeleteDirectorAsync(Guid directorId)
    {
        var director = await _dataAppContext.Directors.FindAsync(directorId);
        if (director == null)
        {
            return false;
        }

        _dataAppContext.Directors.Remove(director);
        await _dataAppContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<Director>> GetAllDirectorsAsync()
    {
        var directors = await _dataAppContext.Directors.ToListAsync();
        return directors;
    }

    public async Task<Director?> UpdateDirectorAsync(Guid directorId, Director updateDirector)
    {
        var director = await _dataAppContext.Directors.FindAsync(directorId);
        if (director == null)
        {
            return null;
        }

        director.Name = updateDirector.Name;
        await _dataAppContext.SaveChangesAsync();
        return director;
    }
}