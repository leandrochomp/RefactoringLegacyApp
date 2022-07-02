using Refactoring.LegacyService.Models;

namespace Refactoring.LegacyService.DataAccess
{
    public interface ICandidateDataAccess
    {
        void AddCandidate(Candidate candidate);
    }
}