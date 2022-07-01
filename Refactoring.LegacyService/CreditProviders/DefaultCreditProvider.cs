using Refactoring.LegacyService.Models;
using Refactoring.LegacyService.Services;

namespace Refactoring.LegacyService.CreditProviders
{
    public class DefaultCreditProvider : ICreditProvider
    {
        private readonly ICandidateCreditService _candidateCreditService;

        public DefaultCreditProvider(ICandidateCreditService candidateCreditService)
        {
            _candidateCreditService = candidateCreditService;
        }

        public (bool RequireCreditCheck, int Credit) GetCredits(Candidate candidate)
        {
            var credit = _candidateCreditService.GetCredit(candidate.Firstname, candidate.Surname, candidate.DateOfBirth);
            return (true, credit);
        }

        public string NameRequirement { get; } = string.Empty;
    }
}
