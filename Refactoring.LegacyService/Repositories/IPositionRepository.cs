using Refactoring.LegacyService.Models;

namespace Refactoring.LegacyService.Repositories
{
    public interface IPositionRepository
    {
        Position GetById(int id);
    }
}