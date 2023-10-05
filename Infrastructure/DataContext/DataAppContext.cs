using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataContext;

public class DataAppContext: DbContext
{
    public DataAppContext(){}
    public DataAppContext(DbContextOptions<DataAppContext> options): base(options){}
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
        {
            options.UseNpgsql("DefaultConnection");
        }
    }
    
    public DbSet<Movie> Movies { get; set; }
}