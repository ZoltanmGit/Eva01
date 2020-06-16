using System;
using System.Threading.Tasks;

namespace Asteroid.Persistence
{
    public interface IAsteroidDataAccess
    {
        Task<AsteroidTable> LoadAsync(String path);
        Task SaveAsync(String path, AsteroidTable table);
    }
}
