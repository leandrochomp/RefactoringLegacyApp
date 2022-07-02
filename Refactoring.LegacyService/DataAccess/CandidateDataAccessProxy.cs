using Refactoring.LegacyService.Models;
using Refactoring.LegacyService.Services;

namespace Refactoring.LegacyService.DataAccess
{
    public class CandidateDataAccessProxy : ICandidateDataAccess
    {
        public void AddCandidate(Candidate candidate)
        {
            CandidateDataAccess.AddCandidate(candidate);
        }
    }
}